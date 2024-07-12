using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Data.Dto;

public class UpdateContasPagasDto
{

    [Required]
    public int ContasPagarRecorrenciaId { get; set; }

    [Required]
    public string FormaPagamento { get; set; }

    [Required]
    public DateTime DataPagamento { get; set; }
}
