using Microsoft.AspNetCore.Mvc;
using GoodJobsEmployed.API.BL;

namespace GoodJobsEmployed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatoController : ControllerBase
{
    private readonly CandidatoAnalisis _candidatoBL;

    public CandidatoController(CandidatoAnalisis candidatoBL)
    {
        _candidatoBL = candidatoBL;
    }

    [HttpPost("AnalizarCV/{idPerfil}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AnalizarCV(int idPerfil, IFormFile cvFile)
    {
        var result = await _candidatoBL.AnalizarCVPdfAsync(idPerfil, cvFile);

        if (result.Correct)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}
