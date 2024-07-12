using ERP_Financeiro_API.Models;
using System.ComponentModel.DataAnnotations;

namespace ERP_Financeiro_API.Data.Dto;

public class CreateUsuarioDto
{
    public string? ImageProfile { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    public string? Cpf { get; set; }

    public string? Cnpj { get; set; }

    public Perfil Perfil { get; set; } = Perfil.Assistente; // Cadastra o valor default 'Assistente'

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string PasswordConfirmation { get; set; }

}
