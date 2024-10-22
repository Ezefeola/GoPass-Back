namespace GoPass.Application.Utilities.Mappers
{
    public interface ICustomAutoMapper
    {
        TDestination Map<TSource, TDestination>(TSource sourceObject);
        TDestination Map<TSource, TDestination>(TSource sourceObject, TDestination destinationObject);
    }
}