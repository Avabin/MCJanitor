namespace MCJanitor.Web.Features.StorageSystem.ValueObjects;

[GenerateSerializer, Immutable]
public readonly record struct Coordinate(
    [property: Id(0)] int X,
    [property: Id(1)] int Y,
    [property: Id(2)] int Z) : IValueObject;