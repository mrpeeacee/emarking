using HmacHashing;

namespace TokenLibrary.EncryptDecrypt.Hmac
{
    public static class Hashing
    {
        public static string? GetHash(string key, string text)
        {
            string? hash = null;
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(text))
            {
                hash = HmacHash.Encrypt(key, text);
            }
            return hash;
        }
    }
}
