using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Saras.eMarking.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool IsLocalHost(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            return string.Equals(ip, "127.0.0.1", StringComparison.Ordinal) || string.Equals(ip, "::1", StringComparison.Ordinal);
        }

        /// <summary>
        /// Very basic check to try and remove the port from any ipv4 or ipv6 address.
        /// </summary>
        /// <returns>ip address without port</returns>
        public static string ToAddress(this string ip)
        {
            if (string.IsNullOrEmpty(ip) || string.Equals(ip, "::1"))
                return ip;

            string[] parts = ip.Split(new[] { ':' }, 9);
            if (parts.Length == 2)  // 1.2.3.4:port
                return parts[0];
            if (parts.Length > 8) // 1:2:3:4:5:6:7:8:port
                return string.Join(":", parts.Take(8));

            return ip;
        }

        public static bool IsPrivateNetwork(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            if (ip.IsLocalHost())
                return true;

            // 10.0.0.0 – 10.255.255.255 (Class A)
            if (ip.StartsWith("10."))
                return true;

            // 172.16.0.0 – 172.31.255.255 (Class B)
            if (ip.StartsWith("172."))
            {
                for (int range = 16; range < 32; range++)
                {
                    if (ip.StartsWith("172." + range + "."))
                        return true;
                }
            }

            // 192.168.0.0 – 192.168.255.255 (Class C)
            return ip.StartsWith("192.168.");
        }

        public static string TrimScript(this string script)
        {
            if (string.IsNullOrEmpty(script))
                return script;

            return script
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("  ", " ");
        }

        public static string GetNewToken()
        {
            return GetRandomString(40);
        }

        public static string GetRandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "length cannot be less than zero.");

            if (String.IsNullOrEmpty(allowedChars))
                throw new ArgumentException("allowedChars may not be empty.");

            const int byteSize = 0x100;
            char[] allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
                throw new ArgumentException($"allowedChars may contain no more than {byteSize} characters.");

            var result = new StringBuilder();
            byte[] buf = new byte[128];

            while (result.Length < length)
            {
                RandomNumberGenerator.Fill(buf);
                for (int i = 0; i < buf.Length && result.Length < length; ++i)
                {
                    int outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                    if (outOfRangeStart <= buf[i])
                        continue;
                    result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                }
            }

            return result.ToString();
        } 

        public static bool IsNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsNumber(value[i]))
                    continue;

                if (i == 0 && value[i] == '-')
                    continue;

                return false;
            }

            return true;
        }

        public static bool IsValidFieldName(this string value)
        {
            if (value == null || value.Length > 25)
                return false;

            return IsValidIdentifier(value);
        }

        public static bool IsValidIdentifier(this string value)
        {
            if (value == null)
                return false;

            for (int index = 0; index < value.Length; index++)
            {
                if (!char.IsLetterOrDigit(value[index]) && value[index] != '-')
                    return false;
            }

            return true;
        }

        public static string ToSaltedHash(this string password, string salt)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            HMACSHA256 hashStrategy = new();
            if (hashStrategy.Key.Length == saltBytes.Length)
            {
                hashStrategy.Key = saltBytes;
            }
            else if (hashStrategy.Key.Length < saltBytes.Length)
            {
                byte[] keyBytes = new byte[hashStrategy.Key.Length];
                Buffer.BlockCopy(saltBytes, 0, keyBytes, 0, keyBytes.Length);
                hashStrategy.Key = keyBytes;
            }
            else
            {
                byte[] keyBytes = new byte[hashStrategy.Key.Length];
                for (int i = 0; i < keyBytes.Length;)
                {
                    int len = Math.Min(saltBytes.Length, keyBytes.Length - i);
                    Buffer.BlockCopy(saltBytes, 0, keyBytes, i, len);
                    i += len;
                }
                hashStrategy.Key = keyBytes;
            }

            byte[] result = hashStrategy.ComputeHash(passwordBytes);
            return Convert.ToBase64String(result);
        }

        public static string ToDelimitedString(this IEnumerable<string> values, string delimiter = ",")
        {
            if (string.IsNullOrEmpty(delimiter))
                delimiter = ",";

            var sb = new StringBuilder();
            foreach (string i in values)
            {
                if (sb.Length > 0)
                    sb.Append(delimiter);

                sb.Append(i);
            }

            return sb.ToString();
        }

        public static string[] FromDelimitedString(this string value, string delimiter = ",")
        {
            if (string.IsNullOrEmpty(value))
                return Array.Empty<string>();

            if (string.IsNullOrEmpty(delimiter))
                delimiter = ",";

            return value.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        public static string ToLowerUnderscoredWords(this string value)
        {
            var builder = new StringBuilder(value.Length + 10);
            for (int index = 0; index < value.Length; index++)
            {
                char c = value[index];
                if (char.IsUpper(c))
                {
                    if (index > 0 && value[index - 1] != '_')
                        builder.Append('_');

                    builder.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static bool AnyWildcardMatches(this string value, IEnumerable<string> patternsToMatch, bool ignoreCase = false)
        {
            if (ignoreCase)
                value = value.ToLowerInvariant();

            return patternsToMatch.Any(pattern => CheckForMatch(pattern, value, ignoreCase));
        }

        private static bool CheckForMatch(string pattern, string value, bool ignoreCase = true)
        {
            bool startsWithWildcard = pattern.StartsWith("*");
            if (startsWithWildcard)
                pattern = pattern[1..];

            bool endsWithWildcard = pattern.EndsWith("*");
            if (endsWithWildcard)
                pattern = pattern[0..^1];

            if (ignoreCase)
                pattern = pattern.ToLowerInvariant();

            if (startsWithWildcard && endsWithWildcard)
                return value.Contains(pattern);

            if (startsWithWildcard)
                return value.EndsWith(pattern);

            if (endsWithWildcard)
                return value.StartsWith(pattern);

            return value.Equals(pattern);
        }

        public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> stringSelector)
        {
            return values.ToConcatenatedString(stringSelector, string.Empty);
        }

        public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> action, string separator)
        {
            var sb = new StringBuilder();
            foreach (var item in values)
            {
                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(action(item));
            }

            return sb.ToString();
        }

        public static string ReplaceFirst(this string input, string find, string replace)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int i = input.IndexOf(find, StringComparison.Ordinal);
            if (i < 0)
                return input;

            string pre = input[..i];
            string post = input[(i + find.Length)..];
            return string.Concat(pre, replace, post);
        }

        public static IEnumerable<string> SplitLines(this string text)
        {
            return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim());
        }

        public static string StripInvisible(this string s)
        {
            return s.Replace("\r\n", " ").Replace('\n', ' ').Replace('\t', ' ');
        }

        public static string NormalizeLineEndings(this string text, string lineEnding = null)
        {
            if (string.IsNullOrEmpty(lineEnding))
                lineEnding = Environment.NewLine;

            text = text.Replace("\r\n", "\n");
            if (lineEnding != "\n")
                text = text.Replace("\r\n", lineEnding);

            return text;
        }

        public static string Truncate(this string text, int keep)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string buffer = NormalizeLineEndings(text);
            if (buffer.Length <= keep)
                return buffer;

            return string.Concat(buffer[..(keep - 3)], "...");
        }

        public static string TypeName(this string typeFullName)
        {
            return typeFullName?.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public static string ToLowerFiltered(this string value, char[] charsToRemove)
        {
            var builder = new StringBuilder(value.Length);

            for (int index = 0; index < value.Length; index++)
            {
                char c = value[index];
                if (char.IsUpper(c))
                    c = char.ToLowerInvariant(c);

                bool includeChar = true;
                for (int i = 0; i < charsToRemove.Length; i++)
                {
                    if (charsToRemove[i] == c)
                    {
                        includeChar = false;
                        break;
                    }
                }

                if (includeChar)
                    builder.Append(c);
            }

            return builder.ToString();
        }

        public static string[] SplitAndTrim(this string s, char[] separator)
        {
            if (s.IsNullOrEmpty())
                return Array.Empty<string>();

            string[] result = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();

            return result;

        }

        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }             
    }
}
