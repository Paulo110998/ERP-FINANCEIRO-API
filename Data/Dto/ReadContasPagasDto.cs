namespace ERP_Financeiro_API.Data.Dto;

public class ReadContasPagasDto
{

    public int Id { get; set; }

    public int ContasPagarRecorrenciaId { get; set; }

    public string FormaPagamento { get; set; }

    public DateTime DataPagamento { get; set; }

    public string CreatedByUserId { get; set; }


}