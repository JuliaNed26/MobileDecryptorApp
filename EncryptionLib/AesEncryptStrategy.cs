using System.Security.Cryptography;
using System.Text;

namespace EncryptionLib;
public class AesEncryptStrategy : IEncryptStrategy
{
	private readonly string _key;

	public AesEncryptStrategy(string key)
	{
		_key = key;
	}

	public string Decrypt(string encryptedText)
	{
		byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.UTF8.GetBytes(_key);
			aes.IV = new byte[aes.BlockSize / 8];
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			ICryptoTransform decryptor = aes.CreateDecryptor();

			using (MemoryStream ms = new MemoryStream(encryptedBytes))
			{
				using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader sr = new StreamReader(cs))
					{
						return sr.ReadToEnd();
					}
				}
			}
		}
	}

	public string Encrypt(string text)
	{
		byte[] textBytes = Encoding.UTF8.GetBytes(text);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.UTF8.GetBytes(_key);
			aes.IV = new byte[aes.BlockSize / 8];
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			ICryptoTransform encryptor = aes.CreateEncryptor();

			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
				{
					cs.Write(textBytes, 0, textBytes.Length);
					cs.FlushFinalBlock();

					byte[] encryptedBytes = ms.ToArray();

					return Convert.ToBase64String(encryptedBytes);
				}
			}
		}
	}

	public static string GenerateKey()
	{
		byte[] key = new byte[16]; 
		using (var generator = RandomNumberGenerator.Create())
		{
			generator.GetBytes(key);
		}

		return Convert.ToBase64String(key);
	}
}
