using System.Linq.Expressions;
using LMS.Domain.Abstractions.Specifications;
using LMS.Domain.Entities.HttpEntities;

namespace LMS.Infrastructure.Specifications.Authentication.Imgur
{
    public class ImgurTokenSpecification : ISpecification<ImgurToken>
    {
        public Expression<Func<ImgurToken, bool>>? Criteria => null;

        public List<string> Includes => [];

        public bool IsTrackingEnabled => true;

        public Expression<Func<ImgurToken, object>>? OrderBy => null;

        public Expression<Func<ImgurToken, object>>? OrderByDescending => null;

        public int? Skip => null;

        public int? Take => null;
    }
}
