using System.Linq.Expressions;

namespace Slackers.Repository;

public interface IRepository
{
    RepositoryMessage<T> Post<T>(T model) where T : IEntity;
    RepositoryMessage<T> Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity;
    RepositoryMessage<T> Get<T>() where T : IEntity;
}