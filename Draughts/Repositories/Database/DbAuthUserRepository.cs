using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Repositories.Database.JoinEnum;

namespace Draughts.Repositories.Database {
    public class DbAuthUserRepository : DbRepository<AuthUser, UserId, DbAuthUser>, IAuthUserRepository {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DbAuthUserRepository(IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "authuser";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.AuthUser);
        private IQueryBuilder GetAuthuserRoleQuery() => GetBaseQuery().SelectAllFrom("authuser_role");

        protected override IQueryBuilder ApplySpec(Specification<AuthUser> spec, IQueryBuilder builder) {
            var joins = spec.RequiredJoins().ToArray();
            if (joins.Contains(PossibleJoins.AuthUserRole)) {
                builder.Join("authuser_role", "authuser.id", "authuser.user_id");
            }
            return base.ApplySpec(spec, builder);
        }

        protected override IReadOnlyList<AuthUser> Parse(IReadOnlyList<DbAuthUser> qs) {
            if (qs.Count == 0) {
                return new List<AuthUser>().AsReadOnly();
            }

            var userRoles = GetAuthuserRoleQuery().Where("user_id").In(qs.Select(q => q.Id)).List<DbAuthUserRole>();
            var roleIds = userRoles.ToLookup(ur => ur.UserId, ur => ur.RoleId);
            var roles = _roleRepository.List(new RoleIdsSpecification(userRoles.Select(ur => ur.RoleId)));
            return qs
                .Select(q => q.ToDomainModel(roles.IntersectBy(roleIds[q.Id], r => r.Id).ToArray()))
                .ToList()
                .AsReadOnly();
        }

        protected override AuthUser Parse(DbAuthUser q) {
            var userRoles = GetAuthuserRoleQuery().Where("user_id").Is(q.Id).List<DbAuthUserRole>();
            var roles = _roleRepository.List(new RoleIdsSpecification(userRoles.Select(ur => ur.RoleId)));
            return q.ToDomainModel(roles);
        }

        public override void Save(AuthUser entity) {
            var obj = DbAuthUser.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
                InsertRoles(entity.Id, entity.Roles.Select(r => r.Id.Id));
            }
            else {
                var oldRoleIds = GetAuthuserRoleQuery().Where("user_id").Is(entity.Id).List<DbAuthUserRole>()
                    .Select(pr => pr.RoleId).ToArray();
                var newRoleIds = entity.Roles.Select(p => p.Id.Id).ToArray();
                var toDelete = oldRoleIds.Except(newRoleIds).ToArray();
                var toAdd = newRoleIds.Except(oldRoleIds);

                GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
                if (toDelete.Any()) {
                    GetBaseQuery().DeleteFrom("authuser_role")
                        .Where("user_id").Is(entity.Id)
                        .And("role_id").In(toDelete)
                        .Execute();
                }
                InsertRoles(entity.Id, toAdd);
            }
        }

        private void InsertRoles(long userId, IEnumerable<long> roleIds) {
            if (!roleIds.Any()) {
                return;
            }
            var values = BuildInsertValues(userId, roleIds);
            GetBaseQuery().InsertInto("authuser_role")
                .Columns("user_id", "role_id").Values(values)
                .Execute();
        }

        private IEnumerable<long> BuildInsertValues(long userId, IEnumerable<long> roleIds) {
            foreach (var roleId in roleIds) {
                yield return userId;
                yield return roleId;
            }
        }
    }
}
