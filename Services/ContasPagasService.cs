using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP_Financeiro_API.Services;

public class ContasPagasService
{
    private readonly EntidadesContext _entidadesContext;

    public ContasPagasService(EntidadesContext context)
    {
        _entidadesContext = context;
    }

    public async Task PagarConta(CreateContasPagasDto contasPagasDto, string userId)
    {
        // Verificando se o Id do usuário é válido
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException("Não foi possível obter o Id do usuário..");
        }

        // Verifica se a conta já foi paga
        bool contaJaPaga = await _entidadesContext.ContasPagas
            .AnyAsync(c => c.ContasPagarRecorrenciaId == contasPagasDto.ContasPagarRecorrenciaId);

        if (contaJaPaga)
        {
            throw new InvalidOperationException("Essa conta já foi paga.");
        }

        // criando uma nova instância de ContasPagas e atribuindo os valores dos Dtos
        ContasPagas novaContaPaga = new ContasPagas()
        {
            ContasPagarRecorrenciaId = contasPagasDto.ContasPagarRecorrenciaId,
            FormaPagamento = contasPagasDto.FormaPagamento,
            DataPagamento = contasPagasDto.DataPagamento,
            CreatedByUserId = userId
        };

        _entidadesContext.ContasPagas.Add(novaContaPaga);
        await _entidadesContext.SaveChangesAsync();
    }
}
