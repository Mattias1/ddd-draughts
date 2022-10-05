using System;

namespace Draughts.Repositories.Misc;

public interface IDbObject<T, TEntity> : IEquatable<T> where T : IDbObject<T, TEntity>, new() {
}
