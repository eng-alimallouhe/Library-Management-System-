using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Models;
using Org.BouncyCastle.Bcpg;

namespace LMS.Infrastructure.Specifications.Authentication.Token
{
    public class UserWithRoleSpecification : ISpecification<User>
    {
        private readonly Guid _userId;


        public Expression<Func<User, bool>>? Criteria => 
            u => u.UserId == _userId;

        public List<string> Includes => ["Role"];

        public bool IsTrackingEnabled => true;

        public Expression<Func<User, object>>? OrderBy => null;

        public Expression<Func<User, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public UserWithRoleSpecification(
            Guid userId)
        {
            _userId = userId;
        }
    }
}
