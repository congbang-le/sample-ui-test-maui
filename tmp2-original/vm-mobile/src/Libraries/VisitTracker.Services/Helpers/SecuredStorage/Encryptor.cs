using System.Security.Cryptography;

namespace VisitTracker.Services;

public static class Encryptor
{
    private static readonly byte[] Key =
    {
        0x77, 0xC8, 0xC3, 0x07, 0x33, 0xFD, 0xEF,
        0xA5, 0x2A, 0x5A, 0x56, 0x9F, 0xCD, 0x49, 0xC5, 0xCA
    };

    public static string Decrypt(string decryptValue)
    {
        var cipherTextCombined = Convert.FromBase64String(decryptValue);
        string plaintext = null;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            var IV = new byte[aesAlg.BlockSize / 8];
            var cipherText = new byte[cipherTextCombined.Length - IV.Length];
            Array.Copy(cipherTextCombined, IV, IV.Length);
            Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);
            aesAlg.IV = IV;
            aesAlg.Mode = CipherMode.CBC;
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }

    public static string Encrypt(string strValue)
    {
        byte[] encrypted;
        byte[] IV;
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.GenerateIV();
            IV = aesAlg.IV;
            aesAlg.Mode = CipherMode.CBC;
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(strValue);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        var combinedIvCt = new byte[IV.Length + encrypted.Length];
        Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
        Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);
        return Convert.ToBase64String(combinedIvCt);
    }

    public static bool IsBase64(this string base64String)
    {
        if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
            || base64String.Contains(" ") || base64String.Contains("\t") ||
            base64String.Contains("\r") || base64String.Contains("\n"))
            return false;

        try
        {
            Convert.FromBase64String(base64String);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}