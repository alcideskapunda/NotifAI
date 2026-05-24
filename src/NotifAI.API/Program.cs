using Amazon.DynamoDBv2;
using Amazon.SQS;
using FluentValidation;
using NotifAI.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IValidator<Transaction>, TransactionValidator>();

var awsOptions = builder.Configuration.GetAWSOptions();

builder.Services.AddAWSService<IAmazonDynamoDB>(awsOptions);
builder.Services.AddAWSService<IAmazonSQS>(awsOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
