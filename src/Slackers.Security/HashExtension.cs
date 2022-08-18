using System.Security.Cryptography;

namespace Slackers.SecurityQuestions.Security;
public static class HashExtension
{
    private const int SALT_SIZE = 16;

    private const int HASH_SIZE = 20;

    private const int ITERATION = 65;

    public static string Hash(this string secret)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);

        // Create hash
        var pbkdf2 = new Rfc2898DeriveBytes(secret, salt, ITERATION);
        var hash = pbkdf2.GetBytes(HASH_SIZE);

        // Combine salt and hash
        var hashBytes = new byte[SALT_SIZE + HASH_SIZE];
        Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
        Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

        // Convert to base64
        var base64Hash = Convert.ToBase64String(hashBytes);

        // TODO : use a secret store
        return string.Format("$MYHASH$V1${0}${1}", ITERATION, base64Hash);
    }

    private static bool IsHashSupported(string hashString)
    {
        return hashString.Contains("$MYHASH$V1$");
    }

    public static bool VerifyHash(this string value, string hashedValue)
    {
        // Check hash
        if (!IsHashSupported(hashedValue))
        {
            throw new NotSupportedException("The hash type is not supported");
        }

        // Extract iteration and Base64 string
        var splittedHashString = hashedValue.Replace("$MYHASH$V1$", "").Split('$');
        var iterations = int.Parse(splittedHashString[0]);
        var base64Hash = splittedHashString[1];

        // Get hash bytes
        var hashBytes = Convert.FromBase64String(base64Hash);

        // Get salt
        var salt = new byte[SALT_SIZE];
        Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

        // Create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(value, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(HASH_SIZE);

        // Get result
        for (var i = 0; i < HASH_SIZE; i++)
        {
            if (hashBytes[i + SALT_SIZE] != hash[i])
            {
                return false;
            }
        }
        return true;
    }

}
