using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Identity.Models;

namespace LMS.Infrastructure.Specifications.Authentication.Identity
{
    public class UserWithOtpCodeSpecification : ISpecification<User>
    {
        private readonly string _email;
        
        
        public Expression<Func<User, bool>>? Criteria => 
            u => u.Email.ToLower().Trim() == _email.ToLower().Trim();


        public List<string> Includes => ["OtpCode"];

        public bool IsTrackingEnabled => true;

        public Expression<Func<User, object>>? OrderBy => null;

        public Expression<Func<User, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;


        public UserWithOtpCodeSpecification(
            string email)
        {
            _email = email;
        }
    }
}
