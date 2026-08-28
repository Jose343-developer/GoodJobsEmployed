using GoodJobsEmployed.API.Data;
using GoodJobsEmployed.API.ML;
using Microsoft.EntityFrameworkCore;

namespace GoodJobsEmployed.API.BL;

public class PerfilBL
{
    // Ejemplo de cómo se referenciaría la capa de datos (DL / Data) y la capa de modelos (ML)
    public static Result GetAll(AppDbContext context)
    {
        Result result = new Result();
        try
        {
            // Usamos context (DL) para obtener los perfiles
            var perfiles = context.Perfils.ToList();
            
            result.Objects = new List<object>();
            foreach (var perfil in perfiles)
            {
                result.Objects.Add(perfil);
            }
            
            result.Correct = true;
        }
        catch (Exception ex)
        {
            result.Correct = false;
            result.ErrorMessage = ex.Message;
            result.Ex = ex;
        }

        return result;
    }
}
