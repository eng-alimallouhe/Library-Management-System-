using System.Security.Cryptography;
using LMS.Application.Abstractions.Services.Helpers;

namespace LMS.Infrastructure.Services.Helpers
{
    public class RandomGeneratorService : IRandomGeneratorService
    {
        public string GenerateEightDigitsCode()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[6];
                rng.GetBytes(randomBytes);

                int code = BitConverter.ToInt32(randomBytes, 0) % 1000000;
                return Math.Abs(code).ToString("D6");
            }
        }

        public Task<string> GenerateRandomName(string cleanName)
        {
            throw new NotImplementedException();
        }


        public string GenerateRandomPassword(int passwordLength)
        {
            var finalResult = new char[passwordLength];

            var random = new Random();

            var chars = new List<char>();

            chars.AddRange("abcdefghijklmnopqrstuvwxyz");
            chars.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            chars.AddRange("0123456789");
            chars.AddRange("!@#$%^&*_");

            for (int i = 0; i < passwordLength; i++)
            {
                finalResult[i] = chars[random.Next(chars.Count)];
            }

            return new string(finalResult);
        }
    }
}
