using Microsoft.AspNetCore.Mvc;
using GoodJobsEmployed.API.BL;
using ML = GoodJobsEmployed.API.ML;

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

    [HttpGet("GetAll")]
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

    [HttpGet("GetPerfilById/{idPerfil}")]
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

    [HttpPost("AddPerfil")]
    public IActionResult AddPerfil([FromBody] ML.Perfil perfil)
    {
        // Llamamos al método de la capa BL que desarrollaste
        var result = _perfilBL.AddPerfil(perfil);

        if (result.Correct)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpPut("UpdatePerfil/{idPerfil}")]
    public IActionResult UpdatePerfil(int idPerfil, [FromBody] ML.Perfil perfil)
    {
        if (perfil == null)
        {
            return BadRequest(new ML.Result { Correct = false, ErrorMessage = "El cuerpo de la petición no puede estar vacío." });
        }

        perfil.IdPerfil = idPerfil;
        var result = _perfilBL.UpdatePerfil(perfil);

        if (result.Correct)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpDelete("DeletePerfil/{idPerfil}")]
    public IActionResult DeletePerfil(int idPerfil)
    {
        var result = _perfilBL.DeletePerfil(idPerfil);

        if (result.Correct)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpDelete("DeleteSkill/{idPerfil}/{idCaracteristica}")]
    public IActionResult DeleteSkill(int idPerfil, int idCaracteristica)
    {
        var result = _perfilBL.DeleteSkill(idPerfil, idCaracteristica);

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
