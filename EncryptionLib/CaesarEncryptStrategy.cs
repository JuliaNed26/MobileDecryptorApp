using System.Text;

namespace EncryptionLib;

public class CaesarEncryptStrategy : IEncryptStrategy
{
    private const int AlphabetLength = 26;

    private readonly int _shift;

    public CaesarEncryptStrategy(int shift)
    {
        this._shift = shift % AlphabetLength;
    }

    public string Encrypt(string text)
    {
        return TransformText(text, _shift);
    }

    public string Decrypt(string encryptedText)
    {
        return TransformText(encryptedText, -_shift);
    }

    private static string TransformText(string text, int shift)
    {
        var transformed = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            if (!char.IsLetter(c))
            {
                transformed.Append(c);
                continue;
            }

            bool isUppercase = char.IsUpper(c);
            var offset = isUppercase ? 'A' : 'a';
            var index = (c + shift - offset) % AlphabetLength;
            if (index < 0) index += AlphabetLength;

            transformed.Append((char)(offset + index));
        }

        return transformed.ToString();
    }
}
