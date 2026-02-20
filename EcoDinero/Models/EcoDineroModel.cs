using System.Data.Entity;

namespace EcoDinero.Models
{
    public class EcoDineroModel : DbContext
    {
        public EcoDineroModel() : base("name=EcoDineroDB")
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
