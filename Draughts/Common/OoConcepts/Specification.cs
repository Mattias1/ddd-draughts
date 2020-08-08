using System;
using System.Linq;
using System.Linq.Expressions;

namespace Draughts.Common.OoConcepts {
    public abstract class Specification<T> {
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity) {
            Func<T, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);
        public Specification<T> Or(Specification<T> specification) => new OrSpecification<T>(this, specification);
        public Specification<T> Not() => new NotSpecification<T>(this);
    }

    public class AndSpecification<T> : Specification<T> {
        private readonly Specification<T> _left, _right;

        public AndSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);

        public override Expression<Func<T, bool>> ToExpression() {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();
            InvocationExpression invokedRightExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);
            BinaryExpression andExpression = Expression.AndAlso(leftExpression.Body, invokedRightExpression);

            return Expression.Lambda<Func<T, bool>>(andExpression, leftExpression.Parameters.Single());
        }
    }

    public class OrSpecification<T> : Specification<T> {
        private readonly Specification<T> _left, _right;

        public OrSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);

        public override Expression<Func<T, bool>> ToExpression() {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();
            InvocationExpression invokedRightExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);
            BinaryExpression orExpression = Expression.OrElse(leftExpression.Body, invokedRightExpression);

            return Expression.Lambda<Func<T, bool>>(orExpression, leftExpression.Parameters.Single());
        }
    }

    public class NotSpecification<T> : Specification<T> {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification) => _specification = specification;

        public override Expression<Func<T, bool>> ToExpression() {
            Expression<Func<T, bool>> expression = _specification.ToExpression();
            UnaryExpression notExpression = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters.Single());
        }
    }
}
