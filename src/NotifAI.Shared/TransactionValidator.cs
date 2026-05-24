using FluentValidation;

namespace NotifAI.Shared;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O Id da transação é obrigatório.");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("O Id do cliente é obrigatório.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatório.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
        RuleFor(x => x.CreatedAt).NotEmpty().WithMessage("A data de criação é obrigatória.");
    }
}
