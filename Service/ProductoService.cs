using ApiPresupuesto.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.Metadata;
using MySql.Data.MySqlClient;

namespace ApiPresupuesto.Service
{
    public class ProductoService
    {
        private readonly IConfiguration _configuration;
        private readonly string cadenaSql;

        public ProductoService(IConfiguration configuration)
        {
            _configuration = configuration;
            cadenaSql = _configuration.GetConnectionString("CadenaSQL")!;
        }

        public async Task<List<Producto>> GetProductos()
        {
            string query = "sp_getProductos";
            using (var con = new MySqlConnection(cadenaSql))
            {
                var productos = await con.QueryAsync<Producto>(query, commandType: CommandType.StoredProcedure);
                return productos.ToList();
            }
        }

        public async Task<List<Producto>> GetProductosByCategoriaAndSubcategoria(int iDCategoria, int iDSubCategoria)
        {
            string query = "sp_getProductos_ByCategoria_SubCategoria";
            using(var con = new MySqlConnection(cadenaSql))
            {
                var parametros = new DynamicParameters();
                parametros.Add("@iDCategoria", iDCategoria, dbType: DbType.Int32);
                parametros.Add("@iDSubCategoria", iDSubCategoria, dbType: DbType.Int32);
                var productos = await con.QueryAsync<Producto>(query,parametros,commandType: CommandType.StoredProcedure);
                return productos.ToList();
            }
        }
    }
}
