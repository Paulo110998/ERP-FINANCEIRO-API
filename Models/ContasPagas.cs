using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Models;

public class ContasPagas
{
    [Key]
    [Required]  // type -> int(11) NOT NULL
    public int Id { get; set; }

    [Required]
    public int ContasPagarRecorrenciaId { get; set; }
    public virtual ContasPagarRecorrencia ContasPagarRecorrencia { get; set; }

    [Required]
    public string FormaPagamento { get; set; } // -> logtext NOT NULL

    [Required]
    public DateTime DataPagamento { get; set; } // datetime(6) NOT NULL

    // Armazenar o Id de quem pagou a conta
    [Required]
    public string CreatedByUserId { get; set; } // -> varchar(127) NOT NULL

    [ForeignKey("CreatedByUserId")]
    public virtual Usuario CreatedByUser { get; set; }
}
