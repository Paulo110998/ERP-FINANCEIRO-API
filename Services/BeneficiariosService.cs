using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Services;

public class BeneficiariosService
{
    private readonly EntidadesContext _entidadesContext;

    public BeneficiariosService(EntidadesContext entidadesContext)
    {
        _entidadesContext = entidadesContext;
    }

    public async Task CriarBeneficiario(CreateBeneficiariosDto beneficiariosDto, string userId)
    {
        // Verificando se o Id do usuário é válido
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException("Não foi possível obter o Id do usuário autenticado");
        }

        // Criando uma nova instância de Beneficiários e atribuindo os valores dos DTOs
        Beneficiarios novoBeneficiario = new Beneficiarios
        {
            NomeBeneficiario = beneficiariosDto.NomeBeneficiario,
            Cpf_Cnpj = beneficiariosDto.Cpf_Cnpj,
            Referencia = beneficiariosDto.Referencia,
            Tipo = beneficiariosDto.Tipo,
            Descricao = beneficiariosDto.Descricao,
            CreatedByUserId = userId
        };

        // Adicionando a nova conta ao contexto e salvando as alterações no banco de dados
        _entidadesContext.Beneficiarios.Add(novoBeneficiario);
        await _entidadesContext.SaveChangesAsync();

    }
}
