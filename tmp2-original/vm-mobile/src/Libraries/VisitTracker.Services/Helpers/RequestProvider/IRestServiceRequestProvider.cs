namespace VisitTracker.Services;

public interface IRestServiceRequestProvider
{
    string UserAgent { get; set; }

    /// <summary>
    /// Executes a REST API request asynchronously and returns the response as a specified type.
    /// This method allows you to specify the URL, HTTP method (GET, POST, etc.), and optional data to be sent with the request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">URL of the API</param>
    /// <param name="method">HTTP Method to be used</param>
    /// <param name="data">Payload of the API</param>
    /// <returns></returns>
    Task<T> ExecuteAsync<T>(string url, HttpMethod method, object data = null);
}