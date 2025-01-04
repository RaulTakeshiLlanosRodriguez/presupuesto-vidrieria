using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ApiPresupuesto.Service;
using ApiPresupuesto.Models;

namespace ApiPresupuesto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaService _service;

        public CategoriaController(CategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetCategorias")]
        public async Task<ActionResult<List<Categoria>>> getCategorias()
        {
            try
            {
                var categorias = await _service.GetCategorias();
                if (categorias == null || !categorias.Any())
                {
                    return NotFound(new { message = "No se encontraron productos para la categoria y subcategoria especificadas" });
                }
                return Ok(categorias);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
