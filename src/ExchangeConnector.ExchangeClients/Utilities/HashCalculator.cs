using System.Security.Cryptography;
using System.Text;
using static System.String;

namespace ExchangeConnector.ExchangeClients.Utilities;

public static class HashCalculator
{
    public static string CalculateHMACSHA256Hash(string text, string salt)
    {
        var encoding = Encoding.Default;

        var baText2BeHashed = encoding.GetBytes(text);
        var baSalt = encoding.GetBytes(salt);
        using var hasher = new HMACSHA256(baSalt);
        var baHashedText = hasher.ComputeHash(baText2BeHashed);

        var result = Join("", baHashedText.ToList()
            .Select(b => b.ToString("x2")).ToArray());

        return result;
    }
}