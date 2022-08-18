namespace Slackers.Repository;

public class RepositoryMessage<T>
{
    public T? Model { get; set; }

    public IEnumerable<T>? Results { get; set; }

    public RepositoryResponse Response { get; set; }

    public string Message { get; set; } = string.Empty;

    public Exception? Exception { get; set; }

}