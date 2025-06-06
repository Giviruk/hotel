namespace Hotel.Services;

public static class SecureKeyStore
{
    public static bool IsPinEntered { get; set; }
    private static string _keyName = string.Empty;


    public static async Task<bool> HasPrivateKeyAsync()
    {
        if (IsPinEntered)
        {
            try
            {
                var key = await SecureStorage.Default.GetAsync(_keyName);
                if (key is null) return false;
                return true;
            }
            catch (Exception _)
            {
                return false;
            }
        }

        return false;
    }

    public static async Task<bool> HasPrivateKeyAsync(string pin)
    {
        _keyName = pin;
        try
        {
            var key = await SecureStorage.Default.GetAsync(pin);
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

    public static async Task EnterPin(string pin)
    {
        _keyName = pin;
        IsPinEntered = true;
    }
}