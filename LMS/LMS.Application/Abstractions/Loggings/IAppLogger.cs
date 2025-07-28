namespace LMS.Application.Abstractions.Loggings
{
    public interface IAppLogger<TEntity>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception exception);
    }
}
