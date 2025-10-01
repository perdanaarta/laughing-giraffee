namespace Clems.Application.EventHandler;

public class WalletNotFoundException(Guid eventWalletId) : Exception($"Wallet with id {eventWalletId} not found");