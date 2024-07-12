namespace ERP_Financeiro_API.Data.Dto;

public class ReadCategoriasDto
{
    public int Id { get; set; }

    public string TituloCategoria { get; set; }

    public string DescricaoCategoria { get; set; }

    public string CreatedByUserId { get; set; }

    public ICollection<ReadContasPagarRecorrenciaDto> ReadContasPagarDto { get; set; }
}
