using System.Linq.Expressions;

namespace Slackers.Repository;

/// <summary>
/// Manages basic rest operations of a repository
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Inserts or Posts an object into the store
    /// </summary>
    /// <typeparam name="T">Type of object to store</typeparam>
    /// <param name="model">The object</param>
    /// <returns></returns>
    RepositoryMessage<T> Post<T>(T model) where T : IEntity;

    /// <summary>
    /// Gets a collection of objects from repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <returns><see cref="RepositoryMessage{T}"/></returns>
    RepositoryMessage<T> Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity;
    
    /// <summary>
    /// Gets all Objects of a specified type from the repository
    /// </summary>
    /// <typeparam name="T">Type of Model</typeparam>
    /// <returns><see cref="RepositoryMessage{T}"/></returns>
    RepositoryMessage<T> Get<T>() where T : IEntity;
}