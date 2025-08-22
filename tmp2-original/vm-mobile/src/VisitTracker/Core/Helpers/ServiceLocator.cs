namespace VisitTracker;

/// <summary>
/// ServiceLocator is a static class that provides a way to access services in the application.
/// It uses the IServiceProvider interface to resolve services and allows for easy access to them throughout the application.
/// </summary>
public static class ServiceLocator
{
    private static IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes the ServiceLocator with the provided IServiceProvider instance.
    /// This method should be called once at the start of the application to set up the service locator.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the service of the specified type from the service provider.
    /// This method allows for easy access to services registered in the dependency injection container.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetService<T>()
    {
        return (T)_serviceProvider.GetService(typeof(T));
    }
}