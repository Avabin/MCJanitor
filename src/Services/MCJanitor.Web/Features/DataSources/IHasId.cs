namespace MCJanitor.Web.Features.DataSources;

public interface IHasId<out TImpl> where TImpl : IHasId<TImpl>
{
    Guid Id { get; init; }
    
    TImpl WithId(Guid id);
    
    bool IsEmpty => Id == Guid.Empty;
}