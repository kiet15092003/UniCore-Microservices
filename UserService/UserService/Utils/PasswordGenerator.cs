using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserService.Utils
{
    public static class PasswordGenerator
    {
        public static string GenerateSecurePassword(int length = 8)
        {
            const string uppercaseChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";  // Excluding O and I which can be confused
            const string lowercaseChars = "abcdefghijkmnopqrstuvwxyz"; // Excluding l which can be confused with 1
            const string digitChars = "23456789";                      // Excluding 0 and 1 which can be confused
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            
            // Ensure password has at least one character from each group
            var password = new StringBuilder();
            
            // Add one character from each required group
            password.Append(GetRandomChar(uppercaseChars));
            password.Append(GetRandomChar(lowercaseChars));
            password.Append(GetRandomChar(digitChars));
            password.Append(GetRandomChar(specialChars));
            
            // Fill the remaining length with random characters from all groups
            var allChars = uppercaseChars + lowercaseChars + digitChars + specialChars;
            for (int i = 4; i < length; i++)
            {
                password.Append(GetRandomChar(allChars));
            }
            
            // Shuffle the password characters to avoid predictable pattern
            return new string(ShuffleArray(password.ToString().ToCharArray()));
        }
        
        private static char GetRandomChar(string characterSet)
        {
            return characterSet[RandomNumberGenerator.GetInt32(0, characterSet.Length)];
        }
        
        private static char[] ShuffleArray(char[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = RandomNumberGenerator.GetInt32(0, n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return array;
        }
    }
}
