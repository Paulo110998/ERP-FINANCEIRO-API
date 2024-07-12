using ERP_Financeiro_API.Models;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Data.Dto;

public class UpdateBeneficiarioDto
{
    [Required]
    public string NomeBeneficiario { get; set; }

    [Required]
    public string Cpf_Cnpj { get; set; }

    [Required]
    public string Referencia { get; set; }

    [Required]
    public Tipo Tipo { get; set; }

    [Required]
    public string Descricao { get; set; }
}