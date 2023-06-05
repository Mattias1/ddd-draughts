namespace Draughts.Application.Shared.ViewModels;

public class NonceViewModel {
    public string Nonce { get; }

    public NonceViewModel(string nonce) {
        Nonce = nonce;
    }
}
