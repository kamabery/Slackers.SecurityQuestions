namespace Slackers.Repository;

/// <summary>
/// Wraps the repository action with meta data to help consuming class handle results appropriately
/// </summary>
/// <typeparam name="T"></typeparam>
public class RepositoryMessage<T>
{
    /// <summary>
    /// Gets or sets The model.  This is set during a post or update
    /// </summary>
    public T? Model { get; set; }

    /// <summary>
    /// Gets sets the The collection of results.  This is set during a Get
    /// </summary>
    public IEnumerable<T>? Results { get; set; }


    /// <summary>
    /// Results of attempted action (Ok, Error, etc.) meant to be a subset of http responses
    /// </summary>
    public RepositoryResponse Response { get; set; }

    /// <summary>
    /// If there is any additional details to response that might need to be conveyed to user
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Details of any exception that occurred.
    /// </summary>
    public Exception? Exception { get; set; }

}