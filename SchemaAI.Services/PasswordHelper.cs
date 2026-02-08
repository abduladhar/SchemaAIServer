using System.Security.Cryptography;

namespace SchemaAI.Services
{
    public static class PasswordHelper
    {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        public static string GenerateRandomPassword(int length = 12)
        {
            if (length < 6) length = 6; // minimum length

            string allChars = Uppercase + Lowercase + Digits + SpecialChars;
            var password = new char[length];
            using var rng = RandomNumberGenerator.Create();

            for (int i = 0; i < length; i++)
            {
                byte[] randomByte = new byte[1];
                rng.GetBytes(randomByte);
                var randomIndex = randomByte[0] % allChars.Length;
                password[i] = allChars[randomIndex];
            }

            return new string(password);
        }
    }
}
