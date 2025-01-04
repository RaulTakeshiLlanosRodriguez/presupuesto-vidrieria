using ApiPresupuesto.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace ApiPresupuesto.Service
{
    public class PresupuestoService
    {
        private readonly IConfiguration _configuration;
        private readonly string cadenaSql;

        public PresupuestoService(IConfiguration configuration)
        {
            _configuration = configuration;
            cadenaSql = _configuration.GetConnectionString("CadenaSQL")!;
        }

        public async Task<int> insertPresupuesto(Presupuesto presupuesto)
        {
            using(var con = new MySqlConnection(cadenaSql))
            {
                await con.OpenAsync();
                using(var transaction = con.BeginTransaction())
                {
                    try
                    {
                        decimal totalPresupuesto = 0;
                        if (presupuesto.items != null && presupuesto.items.Any())
                        {
                            totalPresupuesto = presupuesto.items.Sum(item => item.total);
                        }
                        var parameters = new DynamicParameters();
                        parameters.Add("p_codigoCliente", presupuesto.codigoCliente);
                        parameters.Add("p_rucCliente", presupuesto.rucCliente);
                        parameters.Add("p_tNombreCliente", presupuesto.tNombreCliente);
                        parameters.Add("p_tDniCliente", presupuesto.tDniCliente);
                        parameters.Add("p_obra", presupuesto.obra);
                        parameters.Add("p_ensamble", presupuesto.ensamble);
                        parameters.Add("p_instalacion", presupuesto.instalacion);
                        parameters.Add("p_entrega", presupuesto.entrega);
                        parameters.Add("p_formaPago", presupuesto.formaPago);
                        parameters.Add("p_retenciones", presupuesto.retenciones);
                        parameters.Add("p_validez", presupuesto.validez);
                        parameters.Add("p_totalPresupuesto", totalPresupuesto);
                        parameters.Add("p_iDPresupuesto", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        string queryPresupuesto = "insert_Presupuesto";
                        await con.ExecuteAsync(queryPresupuesto,parameters,transaction,commandType:CommandType.StoredProcedure);
                        var iDPresupuesto = parameters.Get<int>("p_iDPresupuesto");

                        if(presupuesto.items != null && presupuesto.items.Any())
                        {
                            string queryPresupuestoDetalle = "insert_Presupuesto_Detalle";
                            foreach(var item in presupuesto.items)
                            {
                                var detalleParameters = new DynamicParameters();
                                detalleParameters.Add("p_iDPresupuesto", iDPresupuesto);
                                detalleParameters.Add("p_iDProducto", item.iDProducto);
                                detalleParameters.Add("p_cantidad", item.cantidad);
                                detalleParameters.Add("p_formatoUnidades", item.formatoUnidades);
                                detalleParameters.Add("p_nPrecio", item.nPrecio);
                                detalleParameters.Add("p_total", item.total);
                                detalleParameters.Add("p_descripcion", item.descripcion_item);

                                await con.ExecuteAsync(queryPresupuestoDetalle, detalleParameters, transaction, commandType: CommandType.StoredProcedure);
                            }
                        }
                        transaction.Commit();
                        return iDPresupuesto;
                    }
                    catch {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<Presupuesto> getPresupuesto_ById(int iDPresupuesto)
        {
            string query = "get_Presupuesto_By_Id";
            using(var con = new MySqlConnection(cadenaSql))
            {
                var parametros = new DynamicParameters();
                parametros.Add("@p_iDPresupuesto", iDPresupuesto, dbType: DbType.Int32);
                var presupuesto = await con.QueryFirstOrDefaultAsync<Presupuesto>(query, parametros, commandType: CommandType.StoredProcedure);
                if (presupuesto != null)
                {
                    var detalles = await GetPresupuestoDetalles_ById(iDPresupuesto);
                    presupuesto.items = detalles;
                }

                return presupuesto;
            }
        }

        public async Task<List<Presupuesto_Detalle>> GetPresupuestoDetalles_ById(int iDPresupuesto)
        {
            string query = "get_PresupuestoDetalle_By_Id";
            using (var con = new MySqlConnection(cadenaSql))
            {
                var parametros = new DynamicParameters();
                parametros.Add("@p_iDPresupuesto", iDPresupuesto, dbType: DbType.Int32);

                var detalles = await con.QueryAsync<Presupuesto_Detalle>(
                    query,
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return detalles.ToList();
            }
        }

        public async Task<int> getNumeroPropuesta()
        {
            string query = "get_numeroPresupuesto";
            using(var con = new MySqlConnection(cadenaSql))
            {
                var numero = await con.QueryAsync<int>(query,commandType: CommandType.StoredProcedure);
                return numero.FirstOrDefault();
            }
        }

    }
}
