using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Services;

public class ContasPagarRecorrenciaService
{
    private readonly EntidadesContext _entidadesContext;

    public ContasPagarRecorrenciaService(EntidadesContext entidadesContext)
    {
        _entidadesContext = entidadesContext;
    }

    public async Task CriarContasPagarRecorrencia(CreateContasPagarRecorrenciaDto contasPagarRecorrenciaDto, string userId)
    {
        // Verificar se o Id do usuário é válido
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException("Não foi possível obter o Id do usuário autenticado");
        }

        // Criar uma nova instância de ContasPagarRecorrencia e atribuir os valores do DTO
        ContasPagarRecorrencia novaContaRecorrencia = new ContasPagarRecorrencia
        {
            Fornecedor = contasPagarRecorrenciaDto.Fornecedor,
            DescricaoDespesa = contasPagarRecorrenciaDto.DescricaoDespesa,
            TipoRecorrencia = contasPagarRecorrenciaDto.TipoRecorrencia,
            DataInicioRecorrencia = contasPagarRecorrenciaDto.DataInicioRecorrencia,
            IntervaloRecorrencia = contasPagarRecorrenciaDto.IntervaloRecorrencia,
            DataVencimento = contasPagarRecorrenciaDto.DataVencimento,
            Valor = contasPagarRecorrenciaDto.Valor,
            CategoriasId = contasPagarRecorrenciaDto.CategoriasId,
            BeneficiariosId = contasPagarRecorrenciaDto.BeneficiariosId,
            CreatedByUserId = userId
        };

        // Adicionar a nova conta ao contexto e salvar as alterações no banco de dados
        _entidadesContext.ContasPagarRecorrencia.Add(novaContaRecorrencia);
        await _entidadesContext.SaveChangesAsync();


        // Verifica o tipo de recorrência e realiza a clonagem
        switch (novaContaRecorrencia.TipoRecorrencia)
        {
            case RecurrenceType.Semanal:
                await CloneSemanal(novaContaRecorrencia);
                break;
            //case RecurrenceType.Quinzenal:
            //    await Clone(novaContaRecorrencia);
            //    break;
            case RecurrenceType.Mensal:
                await CloneMensal(novaContaRecorrencia);
                break;
            case RecurrenceType.Anual:
                await CloneAnual(novaContaRecorrencia);
                break;
        }
    }


    //////////////////////// SEMANAL /////////////////////////////////
    private async Task CloneSemanal(ContasPagarRecorrencia conta)
    {
        DateTime dataInicio = conta.DataInicioRecorrencia ?? DateTime.Now;
        DateTime dataVencimento = conta.DataVencimento;

        // Define o número de semanas em um mês
        int weeksInMonth = 4;

        for (int i = 0; i < weeksInMonth; i++)
        {
            // Calcula a nova data de início
            DateTime novaDataInicio = dataInicio.AddDays(7 * (i + 1));

            // Verifica se a nova data de início está após a data de vencimento original
            if (novaDataInicio <= dataVencimento)
            {
                // Clona a conta
                var clone = (ContasPagarRecorrencia)conta.Clone();

                // Define um novo Id para a conta clonada
                clone.Id = 0;

                // Define a nova data de início para a conta clonada
                clone.DataInicioRecorrencia = novaDataInicio;

                // Adiciona a nova conta clonada ao contexto
                _entidadesContext.ContasPagarRecorrencia.Add(clone);
            }
        }

        // Salva as alterações no banco de dados
        await _entidadesContext.SaveChangesAsync();
    }

    //////////////////////// MENSAL /////////////////////////////////
    private async Task CloneMensal(ContasPagarRecorrencia conta)
    {
        DateTime dataInicio = conta.DataInicioRecorrencia ?? DateTime.Now;
        DateTime dataVencimento = conta.DataVencimento;

        // Define o número de meses em um ano
        int monthsInYear = 12;

        for (int i = 0; i < monthsInYear; i++)
        {
            // Calcula a nova data de início
            DateTime novaDataInicio = dataInicio.AddMonths(i + 1);

            // Verifica se a nova data de início está após a data de vencimento original
            if (novaDataInicio <= dataVencimento)
            {
                // Clona a conta
                var clone = (ContasPagarRecorrencia)conta.Clone();

                // Define um novo Id para a conta clonada
                clone.Id = 0;

                // Define a nova data de início para a conta clonada
                clone.DataInicioRecorrencia = novaDataInicio;

                // Adiciona a nova conta clonada ao contexto
                _entidadesContext.ContasPagarRecorrencia.Add(clone);
            }
        }

        // Salva as alterações no banco de dados
        await _entidadesContext.SaveChangesAsync();
    }

    //////////////////////// ANUAL /////////////////////////////////
    private async Task CloneAnual(ContasPagarRecorrencia conta)
    {
        DateTime dataInicio = conta.DataInicioRecorrencia ?? DateTime.Now;
        DateTime dataVencimento = conta.DataVencimento;

        // Clona a conta para o próximo ano
        var clone = (ContasPagarRecorrencia)conta.Clone();

        // Define um novo Id para a conta clonada
        clone.Id = 0;

        // Calcula a nova data de início para a próxima ano
        DateTime novaDataInicio = dataInicio.AddYears(1);

        // Verifica se a nova data de início está após a data de vencimento original
        if (novaDataInicio <= dataVencimento)
        {
            // Define a nova data de início para a conta clonada
            clone.DataInicioRecorrencia = novaDataInicio;

            // Define a nova data de vencimento para a conta clonada
            clone.DataVencimento = dataVencimento.AddYears(1);

            // Adiciona a nova conta clonada ao contexto
            _entidadesContext.ContasPagarRecorrencia.Add(clone);
        }

        // Salva as alterações no banco de dados
        await _entidadesContext.SaveChangesAsync();
    }




}