using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace GoPass.Application.Utilities.Mappers
{
    public class Smappfter : ISmappfter
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new();

        private static readonly ConcurrentDictionary<string, Action<object, object>> _setterCache = new();

        public TDestination Map<TSource, TDestination>(TSource sourceObject)
        {
            var destinationObject = Activator.CreateInstance<TDestination>();
            return Map(sourceObject, destinationObject);
        }

        public TDestination Map<TSource, TDestination>(TSource sourceObject, TDestination destinationObject)
        {
            if (sourceObject == null) throw new ArgumentNullException(nameof(sourceObject));
            if (destinationObject == null) throw new ArgumentNullException(nameof(destinationObject));

            var sourceProperties = GetCachedProperties(typeof(TSource));
            var destinationProperties = GetCachedProperties(typeof(TDestination));

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);

                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    var sourceValue = sourceProperty.GetValue(sourceObject);

                    if (sourceValue != null && destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        var setter = GetCachedSetter(destinationProperty);
                        setter(destinationObject, sourceValue);
                    }
                }
            }

            return destinationObject;
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            return _propertiesCache.GetOrAdd(type, t => t.GetProperties());
        }

        private Action<object, object> GetCachedSetter(PropertyInfo property)
        {
            string key = $"{property.DeclaringType!.FullName}.{property.Name}";

            return _setterCache.GetOrAdd(key, _ =>
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                var instanceCast = Expression.Convert(instance, property.DeclaringType);
                var valueCast = Expression.Convert(value, property.PropertyType);

                var setterCall = Expression.Call(instanceCast, property.GetSetMethod()!, valueCast);

                return Expression.Lambda<Action<object, object>>(setterCall, instance, value).Compile();
            });
        }
    }
}
