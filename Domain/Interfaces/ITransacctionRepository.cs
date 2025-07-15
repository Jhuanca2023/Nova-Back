using System.Transactions;

namespace Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> GetById(int id);
    Task<IEnumerable<Transaction>> GetAll();
    Task<IEnumerable<Transaction>> GetByOrder(int orderId);
    Task<Transaction> Add(Transaction entity);
    Task Update(Transaction entity);
    Task Delete(int id);
}