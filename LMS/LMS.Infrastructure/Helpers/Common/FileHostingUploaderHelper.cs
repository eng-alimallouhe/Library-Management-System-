using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Settings;
using LMS.Common.Enums;
using LMS.Common.Results;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Helpers.Common
{
    public class FileHostingUploaderHelper : IFileHostingUploaderHelper
    {
        private readonly Cloudinary _cloudinary;

        public FileHostingUploaderHelper(
            IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }


        public async Task<Result<string>> UploadImageAsync(byte[] imageBytes, string? fileName)
        {
            using var stream = new MemoryStream(imageBytes);

            if (fileName == null)
            {
                fileName = Guid.NewGuid().ToString() + ".png";
            }

            if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".png";
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
                AccessMode = "public"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Result<string>.Success($"{uploadResult.SecureUrl.AbsoluteUri}", $"{ResponseStatus.UPLOAD_SUCCESS}");
            }

            return Result<string>.Failure($"{ResponseStatus.UPLOAD_FAILED}");
        }



        public async Task<Result<string>> UploadPdfAsync(byte[] pdfBytes, string? fileName)
        {
            using var stream = new MemoryStream(pdfBytes);

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Guid.NewGuid().ToString() + ".pdf";
            }
            else if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".pdf";
            }

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
                AccessMode = "public",
                Type = "upload",
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // أضف بارامتر inline للرابط
                var inlineUrl = uploadResult.SecureUrl.AbsoluteUri + "?response-content-disposition=inline";

                return Result<string>.Success(inlineUrl, $"{ResponseStatus.UPLOAD_SUCCESS}");
            }

            return Result<string>.Failure($"{ResponseStatus.UPLOAD_FAILED}");
        }

    }
}
