using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Models;

namespace LMS.Infrastructure.Specifications.Authentication.Identity
{
    public class UserWithRefreshTokenSpecification : ISpecification<User>
    {
        private readonly Guid _userId;

        public Expression<Func<User, bool>>? Criteria => 
            u => u.UserId == _userId;

        public List<string> Includes => ["RefreshToken"];

        public bool IsTrackingEnabled => false;


        public Expression<Func<User, object>>? OrderBy => null;


        public Expression<Func<User, object>>? OrderByDescending => null;


        public int? Skip => null;

        public int? Take => null;

        public UserWithRefreshTokenSpecification(
            Guid userId)
        {
            _userId = userId;
        }
    }
}
