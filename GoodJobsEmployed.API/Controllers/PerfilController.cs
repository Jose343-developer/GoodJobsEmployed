using Microsoft.AspNetCore.Mvc;
using GoodJobsEmployed.API.BL;

namespace GoodJobsEmployed.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfilController : ControllerBase
{
    private readonly Perfil _perfilBL;

    // Inyectamos la capa de Business Logic (BL)
    public PerfilController(Perfil perfilBL)
    {
        _perfilBL = perfilBL;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        // Llamamos al método de la capa BL que desarrollaste
        var result = _perfilBL.GetAll();

        if (result.Correct)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpGet("{idPerfil}")]
    public IActionResult GetByIdPerfil(int idPerfil)
    {
        // Llamamos al método de la capa BL que desarrollaste
        var result = _perfilBL.GetByIdPerfil(idPerfil);

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
