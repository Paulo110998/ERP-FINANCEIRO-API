using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Data.Dto;

public class UpdateCategoriasDto
{
    [Required]
    public string TituloCategoria { get; set; }

    public string? DescricaoCategoria { get; set; }

}
