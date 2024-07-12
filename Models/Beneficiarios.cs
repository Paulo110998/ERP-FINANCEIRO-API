using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Models;

public enum Tipo
{
    PF, // 0
    PJ // 1

}

public class Beneficiarios
{
    [Key]
    [Required]

    public int Id { get; set; }

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

    // Armazenar o Id de quem criou o Beneficiário
    [Required]
    public string CreatedByUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    public virtual Usuario CreatedByUser { get; set; }

    public virtual ICollection<ContasPagarRecorrencia> ContasPagarRecorrencia { get; set; }
}
