using System.Security.Cryptography;
using System.Text;

namespace Hotel.Services;

public class RsaEncryptionService : IRsaEncryptionService
{
    private RSA _rsa;


    public string Encrypt(string plaintext)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Шифруем AES-ключ RSA
        var aesKeyEncrypted = _rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);

        var result = new
        {
            Key = Convert.ToBase64String(aesKeyEncrypted),
            IV = Convert.ToBase64String(aes.IV),
            Data = Convert.ToBase64String(cipherBytes)
        };

        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    public string Decrypt(string encryptedPackage)
    {
        var obj = System.Text.Json.JsonSerializer.Deserialize<EncryptedPackage>(encryptedPackage);
        var key = _rsa.Decrypt(Convert.FromBase64String(obj.Key), RSAEncryptionPadding.Pkcs1);
        var iv = Convert.FromBase64String(obj.IV);
        var data = Convert.FromBase64String(obj.Data);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(data, 0, data.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public async Task LoadKey()
    {
        _rsa = RSA.Create();
        var key = await SecureKeyStore.LoadPrivateKeyAsync();
        _rsa.ImportFromPem(key.ToCharArray());
    }

    private class EncryptedPackage
    {
        public string Key { get; set; } = string.Empty;
        public string IV { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
