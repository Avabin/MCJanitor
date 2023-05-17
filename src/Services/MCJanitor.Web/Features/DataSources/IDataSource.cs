namespace MCJanitor.Web.Features.DataSources;

public interface IDataSource<T> where T : struct, IHasId<T>
{
    Task InsertAsync(T item);
    Task UpdateAsync(T item);
    Task DeleteAsync(T item);
    Task<T> GetAsync(Guid id);
    Task<List<T>> GetAsync();
}