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
           var context = _context;
            
            var query = context.PerfilGetAlls.FromSqlInterpolated($"Exec PerfilGetAll").ToList();
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

    public ML.Result GetByIdPerfil(int idPerfil){

            ML.Result result = new ML.Result();
            try{

                var context = _context;
                var query = context.PerfilGetByIdPerfils.FromSqlInterpolated($"Exec GetPerfilById @idPerfil = {idPerfil}").ToList();


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



            }catch(Exception ex){

                result.Correct = false;
                result.Ex = ex;
                result.ErrorMessage = ex.Message;
                return result;
            }
    }

    public ML.Result AddPerfil(ML.Perfil perfil)
    {
        ML.Result result = new ML.Result();
        try
        {
            if (perfil == null || string.IsNullOrWhiteSpace(perfil.Nombre))
            {
                result.Correct = false;
                result.ErrorMessage = "El nombre del perfil es requerido.";
                return result;
            }

            var context = _context;
            // Ejecutamos el Stored Procedure cadPerfil:
            // @opcion = 1 (opción estática para insertar)
            // @idPerfil = NULL (autogenerado por la BD)
            // @nombrePerfil = string
            int rowsAffected = context.Database.ExecuteSqlInterpolated($"Exec cadPerfil @opcion = 1, @idPerfil = NULL, @nombrePerfil = {perfil.Nombre}");

            if (rowsAffected > 0)
            {
                result.Correct = true;
            }
            else
            {
                result.Correct = false;
                result.ErrorMessage = "No se pudo insertar el perfil";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Correct = false;
            result.Ex = ex;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public ML.Result UpdatePerfil(ML.Perfil perfil)
    {
        ML.Result result = new ML.Result();
        try
        {
            if (perfil == null || perfil.IdPerfil <= 0)
            {
                result.Correct = false;
                result.ErrorMessage = "El identificador del perfil es requerido para la actualización.";
                return result;
            }

            var context = _context;

            // Si el nombre viene vacío o solo espacios, recuperamos el nombre actual de la BD para conservarlo
            string nombreActualizado = perfil.Nombre;
            if (string.IsNullOrWhiteSpace(nombreActualizado))
            {
                var perfilExistente = context.Perfils.AsNoTracking().FirstOrDefault(p => p.IdPerfil == perfil.IdPerfil);
                if (perfilExistente == null)
                {
                    result.Correct = false;
                    result.ErrorMessage = "No se encontró el perfil a actualizar.";
                    return result;
                }
                nombreActualizado = perfilExistente.Nombre;
            }

            // Ejecutamos el Stored Procedure cadPerfil con @opcion = 2 (Update):
            int rowsAffected = context.Database.ExecuteSqlInterpolated($"Exec cadPerfil @opcion = 2, @idPerfil = {perfil.IdPerfil}, @nombrePerfil = {nombreActualizado}");

            if (rowsAffected > 0)
            {
                result.Correct = true;
            }
            else
            {
                result.Correct = false;
                result.ErrorMessage = "No se pudo actualizar el perfil o no se encontró el registro.";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Correct = false;
            result.Ex = ex;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public ML.Result DeletePerfil(int idPerfil)
    {
        ML.Result result = new ML.Result();
        try
        {
            if (idPerfil <= 0)
            {
                result.Correct = false;
                result.ErrorMessage = "El identificador del perfil debe ser mayor a 0.";
                return result;
            }

            var context = _context;

            // Primero eliminamos en cascada las características asociadas en Perfil_Caracteristica
            context.Database.ExecuteSqlInterpolated($"DELETE FROM Perfil_Caracteristica WHERE idPerfil = {idPerfil}");

            // Ejecutamos el Stored Procedure cadPerfil con @opcion = 3 (Delete)
            int rowsAffected = context.Database.ExecuteSqlInterpolated($"Exec cadPerfil @opcion = 3, @idPerfil = {idPerfil}, @nombrePerfil = ''");

            if (rowsAffected > 0)
            {
                result.Correct = true;
            }
            else
            {
                result.Correct = false;
                result.ErrorMessage = "No se encontró el perfil a eliminar.";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Correct = false;
            result.Ex = ex;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }
}
