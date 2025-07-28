namespace LMS.Application.Abstractions.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        
        Task CommitTransactionAsync();
        
        Task RollbackTransactionAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
