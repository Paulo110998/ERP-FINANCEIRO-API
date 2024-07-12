using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Data.Dto;

public class ReadBeneficiariosDto
{
    public int Id { get; set; }

    public string NomeBeneficiario { get; set; }

    public string Cpf_Cnpj { get; set; }

    public string Referencia { get; set; }

    public Tipo Tipo { get; set; }

    public string Descricao { get; set; }

    public string CreatedByUserId { get; set; }

    public ICollection<ReadContasPagarRecorrenciaDto> ReadContasPagarRecorrenciaDto { get; set; }
}