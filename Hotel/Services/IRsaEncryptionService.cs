namespace Hotel.Services;

public interface IRsaEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string base64Cipher);

    Task LoadKey();
}