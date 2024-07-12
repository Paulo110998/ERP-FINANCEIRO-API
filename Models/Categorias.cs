using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Models;

public class Categorias
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string? TituloCategoria { get; set; }

    public string? DescricaoCategoria { get; set; }

    // Armazenar o Id de quem criou a categoria
    [Required]
    public string CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual Usuario CreatedByUser { get; set; }

    public virtual ICollection<ContasPagarRecorrencia> ContasAPagar { get; set; }

}
