using LMS.Common.Results;

namespace LMS.Application.Abstractions.ImagesServices
{
    public interface IImageUploader
    {
        Task<Result<string>> UploadImageAsync(byte[] imageStream, string fileName);
    }
}