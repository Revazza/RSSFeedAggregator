using System.Security.Cryptography;
using System.Text;

namespace FetcherConsole;

public static class StringHelper
{
    public static string CalculateHash(string input)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();
        for (var i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }

}