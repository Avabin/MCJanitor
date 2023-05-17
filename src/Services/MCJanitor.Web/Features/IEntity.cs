namespace MCJanitor.Web.Features;

public interface IEntity<out TId> where TId : IValueObject
{
    TId Id { get; }
}