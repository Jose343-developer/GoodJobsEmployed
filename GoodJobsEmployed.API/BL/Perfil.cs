using GoodJobsEmployed.API.Data;
using GoodJobsEmployed.API.ML;
using Microsoft.EntityFrameworkCore;

namespace GoodJobsEmployed.API.BL;

public class Perfil
{

private readonly Data.AppDbContext _context;
public Perfil(Data.AppDbContext context)
{
_context = context;



}




    public ML.Result GetAll()
    {
        ML.Result result = new ML.Result();
        try
        {
            var query = _context.PerfilGetAlls.FromSqlInterpolated($"Exec PerfilGetAll").ToList();
            
            if (query.Count > 0)
            {
                result.Objects = new List<object>();

                // Aquí agrupamos por Perfil usando LINQ
                var perfilesAgrupados = query.GroupBy(p => new { p.idPerfil, p.nombre })
                    .Select(grupo => new 
                    {
                        idPerfil = grupo.Key.idPerfil,
                        nombre = grupo.Key.nombre,
                        
                        // Generamos el arreglo interno de características para este perfil
                        caracteristicas = grupo.Select(c => new 
                        {
                            idCaracteristica = c.idCaracteristica,
                            nombreCaracteristica = c.NombreCaracteristica,
                            descripcionCaracteristica = c.DescripcionCaracteristica,
                            pesoPrioridad = c.pesoPrioridad,
                            criterioEspecifico = c.criterioEspecifico
                        }).ToList()
                    }).ToList();

                // Llenamos result.Objects con nuestra nueva estructura
                foreach (var item in perfilesAgrupados)
                {
                    result.Objects.Add(item);
                }
                
                result.Correct = true;
            }
            else 
            {
                result.Correct = false;
                result.ErrorMessage = "No se encontro ningun perfil";
            }
            
            return result;
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
