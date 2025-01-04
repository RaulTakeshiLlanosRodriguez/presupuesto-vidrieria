using ApiPresupuesto.Models;
using ApiPresupuesto.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPresupuesto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _service;

        public ProductoController(ProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetProductos")]
        public async Task<ActionResult<List<Producto>>> GetProductos()
        {
            try
            {
                var productos = await _service.GetProductos();
                if(productos == null || !productos.Any())
                {
                    return NotFound("No se encontraron productos");
                }
                return Ok(productos);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
            
        }

        [HttpGet]
        [Route("GetProductosByCatAndSubcat")]
        public async Task<ActionResult<List<Producto>>> GetProductosByCatAndSubcat([FromQuery] int iDCategoria, [FromQuery] int iDSubCategoria)
        {
            try
            {
                var productos = await _service.GetProductosByCategoriaAndSubcategoria(iDCategoria, iDSubCategoria);
                if (productos == null || !productos.Any())
                {
                    return NotFound("No se encontraron productos con la categoria y subcategoria especificadas");
                }
                return Ok(productos);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
