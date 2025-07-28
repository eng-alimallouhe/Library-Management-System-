using System.Web;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [MapToApiVersion("1.0")]
        [HttpGet("view-pdf")]
        public async Task<IActionResult> ViewPdf([FromQuery(Name = "url")] string url)
        {
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return BadRequest("Invalid or missing URL");

            using var httpClient = new HttpClient();

            try
            {
                var stream = await httpClient.GetStreamAsync(url);

                var fileStreamResult = new FileStreamResult(stream, "application/pdf")
                {
                    EnableRangeProcessing = true,
                };

                Response.Headers.Add("Content-Disposition", "inline; filename=decision.pdf");

                return fileStreamResult;
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load PDF: {ex.Message}");
            }
        }
    }
}
