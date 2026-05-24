using System.Globalization;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NotifAI.Shared;

namespace NotifAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IAmazonSQS _sqs;
    private readonly IValidator<Transaction> _validator;
    private readonly string _tableName;
    private readonly string _queueUrl;

    public TransactionController(IAmazonDynamoDB dynamoDb, IAmazonSQS sqs, IValidator<Transaction> validator, IConfiguration configuration)
    {
        _dynamoDb = dynamoDb;
        _sqs = sqs;
        _validator = validator;
        _tableName = configuration["AwsResources:TableName"]!;
        _queueUrl = configuration["AwsResources:QueueUrl"]!;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
    {
        var validationResult = await _validator.ValidateAsync(transaction);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        try
        {
            var putItemRequest = new PutItemRequest
            {
                TableName =  _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id",  new AttributeValue { S = transaction.Id } },
                    { "CustomerId",  new AttributeValue { S = transaction.CustomerId } },
                    { "Amount", new AttributeValue { N = transaction.Amount.ToString(CultureInfo.InvariantCulture) } },
                    { "Description", new AttributeValue { S = transaction.Description } },
                    { "CreatedAt", new AttributeValue { S = transaction.CreatedAt.ToString("o") } }
                }
            };
            
            await _dynamoDb.PutItemAsync(putItemRequest);
            
            var messageBody = JsonSerializer.Serialize(transaction);
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody
            };
            
            await _sqs.SendMessageAsync(sendMessageRequest);

            return Accepted(new { Message = "Transação registada e enviada para processamento." });
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Erro interno ao comunicar com a AWS: {e.Message}");
        }
    }
}
