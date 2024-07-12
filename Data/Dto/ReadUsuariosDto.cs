using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Data.Dto;

public class ReadUsuariosDto
{
    public string Id { get; set; }

    public string ImageProfile { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Cpf { get; set; }

    public Perfil Perfil { get; set; }

}
