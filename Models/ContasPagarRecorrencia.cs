using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Models;

public enum RecurrenceType
{
    Semanal, // Weekly
    Quinzenal, //BiWeekly
    Mensal, // Monthly
    Anual, // Year
    Personalizado // Custom
}

public class ContasPagarRecorrencia : ICloneable
{

    [Key]
    [Required]
    public int Id { get; set; } // type -> int(11) NOT NULL

    [Required(ErrorMessage = "Campo 'Fornecedor' é obrigatório")]
    public string Fornecedor { get; set; } // type -> longtext NOT NULL
    [Required]
    public string DescricaoDespesa { get; set; } // type -> longtext NOT NULL

    [Required]
    public RecurrenceType TipoRecorrencia { get; set; }

    [Required]
    public DateTime? DataInicioRecorrencia { get; set; }

    [Required]
    public int? IntervaloRecorrencia { get; set; }

    [Required]
    public DateTime DataVencimento { get; set; } // type -> datetime(6) NOT NULL

    [Required]
    public decimal Valor { get; set; }

    public int? CategoriasId { get; set; } // type -> int(11)
    public virtual Categorias Categorias { get; set; }

    public int? BeneficiariosId { get; set; } // type -> int(11)

    public virtual Beneficiarios Beneficiarios { get; set; }

    //public virtual ICollection<ContasPagas> ContasPagas { get; set; }

    // Armazenar o Id de quem criou a conta
    [Required]
    public string CreatedByUserId { get; set; } // type -> varchar(127) NOT NULL

    [ForeignKey("CreatedByUserId")]
    public virtual Usuario CreatedByUser { get; set; }

    public virtual ICollection<ContasPagas> ContasPagas { get; set; }


    public object Clone()
    {
        return MemberwiseClone();
    }
}
