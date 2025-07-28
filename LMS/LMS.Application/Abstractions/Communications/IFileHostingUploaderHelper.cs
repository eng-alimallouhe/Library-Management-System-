using LMS.Common.Results;

namespace LMS.Application.Abstractions.Communications
{
    public interface IFileHostingUploaderHelper
    {
        Task<Result<string>> UploadImageAsync(byte[] imageBytes, string? FileName);

        Task<Result<string>> UploadPdfAsync(byte[] pdfBytes, string? fileName);
    }
}
