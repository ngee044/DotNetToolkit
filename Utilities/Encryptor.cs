using System.Security.Cryptography;

namespace Utilities;
public class Encryptor
{
	public static (string keyBase64, string ivBase64) CreateKey()
	{
		using var aes = Aes.Create();
		aes.KeySize = 256; // AES-256
		aes.GenerateKey();
		aes.GenerateIV();

		return (
			Converter.ToBase64(aes.Key),
			Converter.ToBase64(aes.IV)
		);
	}

	public static (bool success, byte[]? data, string? error) Encryption(byte[] originalData, string keyBase64, string ivBase64)
	{
		if (originalData == null || originalData.Length == 0)
		{
			return (false, null, "the data field is empty.");
		}
		if (string.IsNullOrEmpty(keyBase64) || string.IsNullOrEmpty(ivBase64))
		{
			return (false, null, "Key or IV is not provided.");
		}

		try
		{
			var key = Converter.FromBase64(keyBase64);
			var iv = Converter.FromBase64(ivBase64);

			using var aes = Aes.Create();
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			using var ms = new MemoryStream();
			using (var encryptor = aes.CreateEncryptor())
			using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
			{
				cs.Write(originalData, 0, originalData.Length);
			}

			return (true, ms.ToArray(), null);
		}
		catch (Exception ex)
		{
			return (false, null, ex.Message);
		}
	}

	public static (bool success, byte[]? data, string? error) Decryption(byte[] encryptedData, string keyBase64, string ivBase64)
	{
		if (encryptedData == null || encryptedData.Length == 0)
		{
			return (false, null, "the data field is empty.");
		}
		if (string.IsNullOrEmpty(keyBase64) || string.IsNullOrEmpty(ivBase64))
		{
			return (false, null, "Key or IV is not provided.");
		}

		try
		{
			var key = Converter.FromBase64(keyBase64);
			var iv = Converter.FromBase64(ivBase64);

			using var aes = Aes.Create();
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;

			using var ms = new MemoryStream(encryptedData);
			using var decryptor = aes.CreateDecryptor();
			using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

			var result = new MemoryStream();
			cs.CopyTo(result);

			return (true, result.ToArray(), null);
		}
		catch (Exception ex)
		{
			return (false, null, ex.Message);
		}
	}
}