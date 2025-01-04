using ApiPresupuesto.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace ApiPresupuesto.Service
{
    public class CategoriaService
    {
        private readonly IConfiguration _configuration;
        private readonly string cadenaSql;

        public CategoriaService(IConfiguration configuration)
        {
            _configuration = configuration;
            cadenaSql = _configuration.GetConnectionString("CadenaSQL")!;
        }

        public async Task<List<Categoria>> GetCategorias()
        {
            string query = "sp_getCategorias";
            string querySubCategorias = "sp_getSubCategorias";
            using (var con = new MySqlConnection(cadenaSql))
            {
               var categorias = await con.QueryAsync<Categoria>(query, commandType:CommandType.StoredProcedure);
                foreach (var categoria in categorias)
                {
                    var parametro = new DynamicParameters();
                    parametro.Add("@iDCategoria", categoria.iDCategoria, dbType: DbType.Int32);
                    var subCategorias = await con.QueryAsync<SubCategoria>(
                        querySubCategorias,
                        parametro,
                        commandType: CommandType.StoredProcedure
                    );

                    categoria.subCategorias = subCategorias.ToList();
                }
                return categorias.ToList();

            }
        }
    }
}
