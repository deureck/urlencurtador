
using System;
using System.Text;

public class Base62Converter
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    // Converte de Base10 (long) para Base62 (string)
    public static string Encode(long number)
    {
        if (number == 0) return Alphabet[0].ToString();
        var result = new StringBuilder();
        while (number > 0)
        {
            result.Insert(0, Alphabet[(int)(number % 62)]);
            number /= 62;
        }
        return result.ToString();
    }

    // Converte de Base62 (string) para Base10 (long)
    public static long Decode(string base62)
    {
        long result = 0;
        foreach (char c in base62)
        {
            result = result * 62 + Alphabet.IndexOf(c);
        }
        return result;
    }
}

// Exemplo de uso:
// string shortUrl = Base62Converter.Encode(123456789); // "8M0kX"
// long id = Base62Converter.Decode("8M0kX"); // 123456789
