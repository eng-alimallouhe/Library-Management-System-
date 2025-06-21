namespace LMS.Domain.Abstractions.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        
        Task CommitTransactionAsync();
        
        Task RollbackTransactionAsync();
    }
}
