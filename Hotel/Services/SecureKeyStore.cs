namespace Hotel.Services;

public static class SecureKeyStore
{
    private static string _keyName;
    
    public static async Task<bool> HasPrivateKeyAsync(string username)
    {
        _keyName = username;
        try
        {
            var key = await SecureStorage.Default.GetAsync(username);
            if (key is null) return false;
            return true;
        }
        catch (Exception _)
        {
            return false;
        }
    }

    public static async Task SavePrivateKeyAsync(string privateKeyPem)
    {
        await SecureStorage.Default.SetAsync(_keyName, privateKeyPem);
    }

    public static async Task<string?> LoadPrivateKeyAsync()
    {
        return await SecureStorage.Default.GetAsync(_keyName);
    }
}