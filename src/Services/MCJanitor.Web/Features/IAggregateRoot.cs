namespace MCJanitor.Web.Features;

public interface IAggregateRoot<out TId> : IEntity<TId> where TId : IValueObject
{
}