namespace EncryptionLib;

public interface IEncryptStrategy
{
    string Encrypt(string text);
    string Decrypt(string encryptedText);
}