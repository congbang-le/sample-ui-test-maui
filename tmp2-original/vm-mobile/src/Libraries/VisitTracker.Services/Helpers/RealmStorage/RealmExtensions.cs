using System.Collections;
using System.Reflection;

namespace VisitTracker.Services;

public static class RealmExtensions
{
    /// <summary>
    ///     Copies the realm properties.
    /// </summary>
    /// <returns>The realm.</returns>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    /// <typeparam name="U">The 1st type parameter.</typeparam>
    public static U Copy<U>(this U first, U second = null) where U : RealmObject
    {
        if (first == null) return default;

        if (second == null) second = (U)Activator.CreateInstance(typeof(U));

        foreach (var property in typeof(U).GetRuntimeProperties())
        {
            var isList = property.PropertyType.IsConstructedGenericType
                         && property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>);

            if (!property.CanRead
                || property.GetIndexParameters().Length > 0
                || (!property.CanWrite && !isList)
               )
                continue;

            var val = property.GetValue(first, null);

            if (isList)
            {
                var baseType = property.PropertyType.GenericTypeArguments[0];

                var secondVal = property.GetValue(second, null);

                var addMethod = secondVal.GetType().GetRuntimeMethod("Add", new[] { baseType });
                MethodInfo copyMethod = null;

                foreach (var item in (IEnumerable)val)
                {
                    if (copyMethod == null)
                    {
                        copyMethod = typeof(RealmExtensions).GetRuntimeMethods()
                            .FirstOrDefault(m => m.Name == "Copy");
                        copyMethod = copyMethod?.MakeGenericMethod(baseType);
                    }

                    if (copyMethod != null)
                    {
                        var copy = copyMethod.Invoke(null, new[] { item, null });
                        addMethod.Invoke(secondVal, new[] { copy });
                    }
                }
            }
            else
            {
                property.SetValue(second, val, null);
            }
        }

        return second;
    }

    public static IEnumerable<U> Copy<U>(this IEnumerable<U> realmObjects)
        where U : RealmObject
    {
        var newRealmObjects = new List<U>();

        foreach (var realmObject in realmObjects) newRealmObjects.Add(realmObject.Copy());

        return newRealmObjects;
    }
}