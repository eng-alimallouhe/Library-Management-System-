using LMS.Common.Results;
using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.Models;

namespace LMS.Application.Abstractions.Authentication
{
    public interface IAuthenticationHelper
    {
        /// <summary>
        /// Generates a verification code of the given <paramref name="codeType"/> for the specified <paramref name="userId"/>,
        /// saves it to the database, and returns the code as a string.
        /// </summary>
        /// <param name="userId">The ID of the user to whom the code belongs.</param>
        /// <param name="codeType">The type of code to generate (e.g., Activation, PasswordReset, OTP).</param>
        /// <returns>A <see cref="Result{String}"/> containing the generated code string if successful.</returns>
        Task<Result<string>> GenerateAndSaveCodeAsync(Guid userId, int codeType);


        /// <summary>
        /// Verifies the provided code against the one stored for the user, based on the specified code type.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the code is being verified.</param>
        /// <param name="code">The code entered by the user.</param>
        /// <param name="codeType">The type of code to verify.</param>
        /// <returns>A <see cref="Result"/> indicating whether the code is valid.</returns>
        Task<Result> VerifyCodeAsync(Guid userId, string code, int codeType);


        /// <summary>
        /// Creates a new customer account using the provided customer details,
        /// and persists it to the database.
        /// </summary>
        /// <param name="request">The customer object containing the account information.</param>
        /// <returns>A <see cref="Result"/> indicating whether the account was successfully created.</returns>
        Task<Result> CreateAndSaveAccountAsync(Customer request);


        /// <summary>
        /// Authenticates the user using the provided credentials and returns
        /// a result indicating success or failure.
        /// </summary>
        /// <param name="user">The user attempting to log in.</param>
        /// <param name="password">The plaintext password to verify.</param>
        /// <returns>A <see cref="Result"/> representing the login result (e.g., success, invalid credentials, locked).</returns>
        Result LogIn(User user, string password);



        /// <summary>
        /// Validates whether a user with the given email can perform an authentication action
        /// based on a stored verification code. This includes checking code validity,
        /// type match, and whether the code has already been verified.
        /// </summary>
        /// <param name="email">The email of the user attempting the action.</param>
        /// <param name="activationType">The expected type of verification (e.g., AccountActivation, PasswordReset).</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the user's ID if the code is valid and matches the action type;
        /// otherwise, a failure result indicating why the action cannot proceed.
        /// If the action type is <c>AccountActivation</c>, the user's account is also marked as verified.
        /// </returns>
        Task<Result<Guid>> CanActivateAuth(string email, int activationType);
    }
}
