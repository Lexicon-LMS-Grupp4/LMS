using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/upload-test")]
[AllowAnonymous]
public class UploadTestController : ControllerBase
{
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Post(CancellationToken ct)
    {
        var form = await Request.ReadFormAsync(ct);
        return Ok(new
        {
            Keys = form.Keys.ToArray(),
            FileCount = form.Files.Count,
            FileName = form.Files.FirstOrDefault()?.FileName,
            FileLength = form.Files.FirstOrDefault()?.Length
        });
    }
}