namespace LMS.Application.Abstractions.Services.Helpers
{
    public interface IRandomGeneratorService
    {
        public string GenerateEightDigitsCode();

        /// <summary>
        /// this method generate a random user name from provided name
        /// </summary>
        /// <param name="cleanName">a string to generate a random name from it</param>
        /// <returns>the generated name</returns>
        public Task<string> GenerateRandomName(string cleanName);


        /// <summary>
        /// generate strong passwword with provided length
        /// </summary>
        /// <param name="passwordLength">the length of the password</param>
        /// <returns>strong password</returns>
        public string GenerateRandomPassword(int passwordLength);
    }
}