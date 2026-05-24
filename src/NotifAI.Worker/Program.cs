using Amazon.SQS;
using NotifAI.Worker;

var builder = Host.CreateApplicationBuilder(args);

var awsOptions = builder.Configuration.GetAWSOptions();

builder.Services.AddAWSService<IAmazonSQS>(awsOptions);

builder.Services.AddHostedService<QueueConsumerWorker>();

var host = builder.Build();
host.Run();
