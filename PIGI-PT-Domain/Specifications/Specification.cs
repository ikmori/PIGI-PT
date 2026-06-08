using System.Linq.Expressions;
using PIGI_PT_Domain.Base;

namespace PIGI_PT_Domain.Specifications
{
    public abstract class Specification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
        public bool IsPagingEnabled { get; protected set; }
        public int PageSize { get; protected set; }
        public int PageIndex { get; protected set; }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
