using ERP_Financeiro_API.Models;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Data.Dto;

public class UpdateContasPagarRecorrenciaDto
{
    [Required(ErrorMessage = "Campo 'Fornecedor' é obrigatório")]
    public string Fornecedor { get; set; }

    [Required]
    public string DescricaoDespesa { get; set; }

    [Required]
    public RecurrenceType TipoRecorrencia { get; set; }

    [Required]
    public DateTime? DataInicioRecorrencia { get; set; }

    [Required]
    public int? IntervaloRecorrencia { get; set; }

    [Required]
    public DateTime DataVencimento { get; set; }

    [Required]
    public decimal Valor { get; set; }

    public int? CategoriasId { get; set; }

    public int? BeneficiariosId { get; set; }
}
