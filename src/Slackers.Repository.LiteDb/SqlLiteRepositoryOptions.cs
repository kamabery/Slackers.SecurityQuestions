namespace Slackers.Repository.LiteDb;
/// <summary>
/// Configuration options for Lite db
/// </summary>
public class SqlLiteRepositoryOptions
{
    /// <summary>
    /// Gets or sets the LiteDb connection string
    /// </summary>
    public string ConnectionString { get; set; } = default!;
}