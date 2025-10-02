namespace Clems.Application.Dtos;

public abstract record TransactionCreateDto(Guid WalletId, decimal In, decimal Out, string Description = "");