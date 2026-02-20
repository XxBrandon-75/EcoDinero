using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoDinero.Models
{
    [Table("Transacciones")]
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(10)]
        public string Tipo { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public virtual Categoria Categoria { get; set; }
    }
}
