using System;

namespace Draughts.Repositories.Database {
    public interface IDbObject<T, TEntity> : IEquatable<T> where T : IDbObject<T, TEntity>, new() {
    }
}
