using Microsoft.AspNetCore.Identity;

namespace ERP_Financeiro_API.Models;

public enum Perfil
{
    Assistente,
    Admin
}

public class Usuario : IdentityUser
{
    public string? ImageProfile { get; set; }

    public string? Cpf { get; set; }

    public string? Cnpj { get; set; }

    public Perfil? Perfil { get; set; }

    public Usuario() : base() { }
}