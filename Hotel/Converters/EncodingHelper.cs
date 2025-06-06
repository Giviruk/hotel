using System.Text;
using System.Text.Json;

public static class EncodingHelper
{
    public static string JsonToBase64<T>(T obj)
    {
        string json = JsonSerializer.Serialize(obj);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(jsonBytes);
    }

    public static T Base64ToJson<T>(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        string json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}