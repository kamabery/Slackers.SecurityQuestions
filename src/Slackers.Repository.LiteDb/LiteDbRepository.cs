using System.Linq.Expressions;
using Microsoft.Extensions.Options;

namespace Slackers.Repository.LiteDb;

/// <summary>
/// An implementation for IRepository specifically for LiteDb
/// </summary>
public class LiteDbRepository : IRepository
{
    private readonly string? _connectionString;

    /// <summary>
    /// Creates an instance of <see cref="LiteDbRepository"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public LiteDbRepository(IOptions<SqlLiteRepositoryOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    /// <inheritdoc />
    public RepositoryMessage<T> Post<T>(T model) where T : IEntity
    {
        try
        {
            using var db = new LiteDB.LiteRepository(_connectionString);
            db.Insert(model);
        }
        catch (Exception e)
        {
            return new RepositoryMessage<T> { Message = e.Message, Response = RepositoryResponse.Error, Exception = e };
        }

        return new RepositoryMessage<T> { Response = RepositoryResponse.Created, Model = model };
    }

    /// <inheritdoc />
    public RepositoryMessage<T> Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity
    {
        try
        {
            using var db = new LiteDB.LiteRepository(_connectionString);
            var result = db.Query<T>().Where(predicate).ToList();
            if (!result.Any())
            {
                return new RepositoryMessage<T> { Message = "Not found", Response = RepositoryResponse.NotFound };
            }

            return new RepositoryMessage<T> { Message = "Ok", Response = RepositoryResponse.Ok, Results = result};

        }
        catch (Exception e)
        {
            return  new RepositoryMessage<T> { Message = e.Message, Response = RepositoryResponse.Error, Exception = e };

        }

    }

    /// <inheritdoc />
    public RepositoryMessage<T> Get<T>() where T : IEntity
    {
        using var db = new LiteDB.LiteRepository(_connectionString);
        var result = db.Query<T>().ToList();
        if (!result.Any())
        {
            return new RepositoryMessage<T> { Message = "Not found", Response = RepositoryResponse.NotFound };
        }

        return new RepositoryMessage<T> { Message = "Ok", Results = result, Response = RepositoryResponse.Ok };
    }

}