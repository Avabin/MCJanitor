using LiteDB.Async;

namespace MCJanitor.Web.Features.DataSources;

public class LiteDbDataSource<T> : IDataSource<T> where T : struct, IHasId<T>
{
    private readonly ILiteDatabaseAsync _liteDatabase;
    private Lazy<ILiteCollectionAsync<T>> _collection => new(() => _liteDatabase.GetCollection<T>(typeof(T).Name));
    private ILiteCollectionAsync<T> Collection => _collection.Value;

    public LiteDbDataSource(ILiteDatabaseAsync liteDatabase)
    {
        _liteDatabase = liteDatabase;
    }
    public async Task InsertAsync(T item) => 
        await Collection.InsertAsync(item);

    public async Task UpdateAsync(T item)
    {
        var exists = await Collection.ExistsAsync(x => x.Id == item.Id);
        
        if (!exists)
        {
            await InsertAsync(item);
            return;
        }
        
        await Collection.UpdateAsync(item);
        
    }

    public async Task DeleteAsync(T item)
    {
        await Collection.DeleteAsync(item.Id);
    }

    public async Task<T> GetAsync(Guid id)
    {
        var existing = await Collection.FindOneAsync(x => x.Id == id);

        if (!existing.IsEmpty) return existing;
        
        existing = existing.WithId(id);
        await InsertAsync(existing);

        return existing;
    }

    public async Task<List<T>> GetAsync()
    {
        var ts = await Collection.FindAllAsync();
        return ts?.ToList() ?? new();
    }
}