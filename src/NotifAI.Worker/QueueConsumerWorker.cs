using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using NotifAI.Shared;

namespace NotifAI.Worker;

public class QueueConsumerWorker : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<QueueConsumerWorker> _logger;
    private readonly string _queueUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public QueueConsumerWorker(IAmazonSQS sqsClient, ILogger<QueueConsumerWorker> logger, IConfiguration configuration)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _queueUrl = configuration["AwsResources:QueueUrl"]!;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 1,
                    WaitTimeSeconds = 20
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                if (response.Messages == null || response.Messages.Count == 0)
                {
                    _logger.LogInformation("Não há mensagens para serem processadas.");
                    continue;
                }

                foreach (var message in response.Messages)
                {
                    try
                    {
                        var transaction = JsonSerializer.Deserialize<Transaction>(message.Body, _jsonOptions);

                        if (transaction != null)
                        {
                            _logger.LogInformation(
                                "Transação recebida! ID: {Id} | Cliente: {CustomerId} | Valor: €{Amount:F2} | Descrição: {Desc}",
                                transaction.Id, transaction.CustomerId, transaction.Amount, transaction.Description);

                            // Aqui simularíamos o disparo do e-mail ou SMS.
                        }

                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };

                        await _sqsClient.DeleteMessageAsync(deleteMessageRequest, stoppingToken);
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError("Erro ao deserializar a mensagem. Formato inválido: {Error}", jsonEx.Message);

                        var deleteInvalidMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        await _sqsClient.DeleteMessageAsync(deleteInvalidMessageRequest, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro inesperado ao processar a mensagem com ID {MessageId}.", message.MessageId);
                    }
                }
            }
            catch (Exception e) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(e, "Erro ao processar a mensagem da fila SQS");
            }

            // Aguarda 10 segundo antes de verificar a fila novamente
            await Task.Delay(10000, stoppingToken);
        }
    }
}
