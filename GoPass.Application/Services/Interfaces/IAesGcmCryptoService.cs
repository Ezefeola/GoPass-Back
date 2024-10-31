namespace GoPass.Application.Services.Interfaces;

public interface IAesGcmCryptoService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
