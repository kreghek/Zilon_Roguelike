using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using Zilon.Tournament.ApiGate.BotManagement;

namespace Zilon.Tournament.ApiGate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        [HttpPost("upload")]
        public async Task<IActionResult> UploadBotAsync()
        {
            var appPath = Environment.GetEnvironmentVariable("APP_PATH");
            var botRootCatalog = Path.Combine(appPath, "bots");

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition,
                    out var contentDisposition);

                var fileName = contentDisposition.FileName.ToString();
                if (!IsValidFileName(fileName))
                {
                    throw new ArgumentException($"Имя имя '{fileName}' не является корректным именем файла.");
                }

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        using (var copyStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(copyStream);

                            copyStream.Seek(0, SeekOrigin.Begin);
                            using (var targetStream = System.IO.File.Create(botRootCatalog))
                            {
                                await copyStream.CopyToAsync(targetStream)
                                                .ConfigureAwait(false);
                            }
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return BadRequest();
        }

        private static bool IsValidFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var invalidCharIndex = fileName.IndexOfAny(invalidChars);
            var hasInvalidChars = invalidCharIndex >= 0;
            return !hasInvalidChars;
        }
    }
}