namespace NotifAI.Shared;

public record Transaction(string Id, string CustomerId, decimal Amount, string Description, DateTime CreatedAt);