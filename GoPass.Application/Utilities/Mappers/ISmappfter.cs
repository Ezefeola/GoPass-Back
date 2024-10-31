namespace GoPass.Application.Utilities.Mappers
{
    public interface ISmappfter
    {
        TDestination Map<TSource, TDestination>(TSource sourceObject);
        TDestination Map<TSource, TDestination>(TSource sourceObject, TDestination destinationObject);
    }
}