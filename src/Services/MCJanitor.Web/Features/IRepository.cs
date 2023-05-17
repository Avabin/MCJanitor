namespace MCJanitor.Web.Features;

public interface IRepository<TAggregateRoot, in TId> where TAggregateRoot : IAggregateRoot<TId> where TId : IValueObject
{
    TAggregateRoot GetById(TId id);
    void Save(TAggregateRoot aggregateRoot);
}