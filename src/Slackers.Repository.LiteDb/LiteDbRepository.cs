using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slackers.Repository.LiteDb;

public class LiteDbRepository : IRepository
{
    private readonly ILogger<LiteDbRepository> _logger;
    private readonly string? _connectionString;

    public LiteDbRepository(IOptions<SqlLiteRepositoryOptions> options, ILogger<LiteDbRepository> logger)
    {
        _logger = logger;
        _connectionString = options.Value.ConnectionString;
    }
    public RepositoryMessage<T> Post<T>(T model) where T : IEntity
    {
        try
        {
            using var db = new LiteDB.LiteRepository(_connectionString);
            db.Insert(model);
        }
        catch (Exception e)
        {
            return LogErrorAndReturn<T>(e);
        }

        return new RepositoryMessage<T> { Response = RepositoryResponse.Created, Model = model };
    }

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
            return LogErrorAndReturn<T>(e);
        }

    }

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

    private RepositoryMessage<T> LogErrorAndReturn<T>(Exception exception)
    {
        _logger.LogError(exception: exception, message: exception.Message);
        return new RepositoryMessage<T> { Message = exception.Message, Response = RepositoryResponse.Error, Exception = exception };
    }

}