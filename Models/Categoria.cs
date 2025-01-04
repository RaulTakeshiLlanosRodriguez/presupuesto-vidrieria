namespace ApiPresupuesto.Models
{
    public class Categoria
    {
        public int iDCategoria { get; set; }
        public string? nombreCategoria { get; set; }
        public List<SubCategoria>? subCategorias { get; set;}
    }
}
