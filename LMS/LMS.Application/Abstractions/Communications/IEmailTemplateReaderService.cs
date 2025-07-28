using LMS.Common.Enums;
using LMS.Domain.Identity.Enums;

namespace LMS.Application.Abstractions.Communications
{
    public interface IEmailTemplateReaderService
    {
        public string? ReadTemplate(Language language, EmailPurpose purpose);
    }
}
