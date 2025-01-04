namespace ApiPresupuesto.Models
{
    public class Presupuesto
    {
        public int? iDPresupuesto { get; set; }
        public string? codigoCliente { get; set; }
        public string? rucCliente { get; set; }
        public string? tNombreCliente { get; set; }
        public string? tDniCliente { get; set; }
        public string? obra { get; set; }
        public string? ensamble { get; set; }
        public string? instalacion { get; set; }
        public string? entrega { get;set;}
        public string? formaPago { get; set; }
        public string? retenciones { get; set; }
        public string? validez { get; set; }
        public int? iNumero { get; set; }
        public decimal? totalPresupuesto { get; set; }
        public decimal? igvPresupuesto { get; set; }
        public decimal? totalIgvPresupuesto { get; set; }
        public List<Presupuesto_Detalle>? items { get; set; }
    }

    public class Presupuesto_Detalle
    {
        public int? iDPresupuestoDetalle { get; set; }
        public int? iDPresupuesto { get; set; }
        public int? iDProducto { get; set; }
        public string? tProducto { get; set; }
        public decimal? cantidad { get; set; }
        public string? formatoUnidades { get; set; }
        public decimal? nPrecio { get; set; }
        public decimal total { get; set; }
        public string? urlImagenProducto { get; set; }
        public string? descripcion_item { get; set; }
    }
}
