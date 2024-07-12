using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Data.Dto;

public class ReadContasPagarRecorrenciaDto
{
    public int Id { get; set; }

    public string Fornecedor { get; set; }

    public string DescricaoDespesa { get; set; }

    public RecurrenceType TipoRecorrencia { get; set; }

    public DateTime? DataInicioRecorrencia { get; set; }

    public int? IntervaloRecorrencia { get; set; }

    public DateTime DataVencimento { get; set; }

    public decimal Valor { get; set; }

    public int? CategoriasId { get; set; }

    public int? BeneficiariosId { get; set; }

    public string CreatedByUserId { get; set; }

    public ICollection<ReadContasPagasDto> ReadContasPagas { get; set; }


}
