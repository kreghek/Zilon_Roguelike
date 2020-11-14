using System;
using System.IO;
using System.Threading.Tasks;
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
            string? appPath = Environment.GetEnvironmentVariable("APP_PATH");
            string botRootCatalog = Path.Combine(appPath, "bots");

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            string boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);
            MultipartReader reader = new MultipartReader(boundary, HttpContext.Request.Body);

            MultipartSection section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                bool hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition,
                    out ContentDispositionHeaderValue contentDisposition);

                string fileName = contentDisposition.FileName.ToString();
                if (!IsValidFileName(fileName))
                {
                    throw new ArgumentException($"Имя имя '{fileName}' не является корректным именем файла.");
                }

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        using (MemoryStream copyStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(copyStream);

                            copyStream.Seek(0, SeekOrigin.Begin);
                            using (FileStream targetStream = System.IO.File.Create(botRootCatalog))
                            {
                                await copyStream.CopyToAsync(targetStream).ConfigureAwait(false);
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
            char[] invalidChars = Path.GetInvalidFileNameChars();
            int invalidCharIndex = fileName.IndexOfAny(invalidChars);
            bool hasInvalidChars = invalidCharIndex >= 0;
            return !hasInvalidChars;
        }
    }
}