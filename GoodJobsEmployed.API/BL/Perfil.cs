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

    public ML.Result GetByIdPerfil(int idPerfil)
    {
        ML.Result result = new ML.Result();
        try
        {
            var context = _context;
            var query = context.PerfilGetByIdPerfils.FromSqlInterpolated($"Exec GetPerfilById @idPerfil = {idPerfil}").ToList();

            result.Objects = new List<object>();

            if (query.Count > 0)
            {
                // Agrupamos por Perfil usando LINQ
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

                foreach (var item in perfilesAgrupados)
                {
                    result.Objects.Add(item);
                }
            }

            result.Correct = true;
            result.ErrorMessage = null;
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

            // Procesar características asociadas si se proporcionaron
            if (perfil.PerfilCaracteristicas != null && perfil.PerfilCaracteristicas.Count > 0)
            {
                foreach (var pc in perfil.PerfilCaracteristicas)
                {
                    int idCaracteristica = pc.IdCaracteristica;
                    if (idCaracteristica <= 0 && pc.Caracteristica != null && !string.IsNullOrWhiteSpace(pc.Caracteristica.NombreCaracteristica))
                    {
                        var nuevaCarac = new Data.Models.Caracteristica
                        {
                            NombreCaracteristica = pc.Caracteristica.NombreCaracteristica.Trim(),
                            DescripcionCaracteristica = pc.Caracteristica.DescripcionCaracteristica?.Trim()
                        };
                        context.Caracteristicas.Add(nuevaCarac);
                        context.SaveChanges();
                        idCaracteristica = nuevaCarac.IdCaracteristica;
                    }

                    if (idCaracteristica > 0)
                    {
                        var relacionExistente = context.PerfilCaracteristicas
                            .FirstOrDefault(r => r.IdPerfil == perfil.IdPerfil && r.IdCaracteristica == idCaracteristica);

                        byte? pesoCalculado = pc.PesoPrioridad.HasValue ? (byte)Math.Clamp(pc.PesoPrioridad.Value, 1, 100) : null;

                        if (relacionExistente == null)
                        {
                            var nuevaRelacion = new Data.Models.PerfilCaracteristica
                            {
                                IdPerfil = perfil.IdPerfil,
                                IdCaracteristica = idCaracteristica,
                                PesoPrioridad = pesoCalculado,
                                CriterioEspecifico = pc.CriterioEspecifico?.Trim()
                            };
                            context.PerfilCaracteristicas.Add(nuevaRelacion);
                        }
                        else
                        {
                            if (pesoCalculado.HasValue)
                            {
                                relacionExistente.PesoPrioridad = pesoCalculado;
                            }
                            if (pc.CriterioEspecifico != null)
                            {
                                relacionExistente.CriterioEspecifico = pc.CriterioEspecifico.Trim();
                            }
                        }

                        var caracExistente = context.Caracteristicas.FirstOrDefault(c => c.IdCaracteristica == idCaracteristica);
                        if (caracExistente != null && pc.Caracteristica != null)
                        {
                            if (!string.IsNullOrWhiteSpace(pc.Caracteristica.NombreCaracteristica))
                            {
                                caracExistente.NombreCaracteristica = pc.Caracteristica.NombreCaracteristica.Trim();
                            }
                            if (pc.Caracteristica.DescripcionCaracteristica != null)
                            {
                                caracExistente.DescripcionCaracteristica = pc.Caracteristica.DescripcionCaracteristica.Trim();
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }

            if (rowsAffected > 0 || (perfil.PerfilCaracteristicas != null && perfil.PerfilCaracteristicas.Count > 0))
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

    public ML.Result DeleteSkill(int idPerfil, int idCaracteristica)
    {
        ML.Result result = new ML.Result();
        try
        {
            if (idPerfil <= 0 || idCaracteristica <= 0)
            {
                result.Correct = false;
                result.ErrorMessage = "Los identificadores del perfil y de la característica deben ser mayores a 0.";
                return result;
            }

            var context = _context;

            // Ejecutamos el Stored Procedure cadCaracteristica con @opcion = 3 (Delete)
            int rowsAffected = context.Database.ExecuteSqlInterpolated($"Exec cadCaracteristica @opcion = 3, @idCaracteristica = {idCaracteristica}, @idPerfil = {idPerfil}");

            if (rowsAffected > 0)
            {
                result.Correct = true;
            }
            else
            {
                result.Correct = false;
                result.ErrorMessage = "No se encontró la característica para eliminar.";
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
