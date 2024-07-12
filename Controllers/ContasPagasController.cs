using AutoMapper;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP_Financeiro_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContasPagasController : ControllerBase
{
    private EntidadesContext _context;
    private IMapper _mapper;
    private ContasPagasService _contasPagasService;

    public ContasPagasController(EntidadesContext context,
        IMapper mapper,
        ContasPagasService contasPagasService)
    {
        _context = context;
        _mapper = mapper;
        _contasPagasService = contasPagasService;
    }

    [HttpPost]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> CadastrarContaPaga([FromBody]
    CreateContasPagasDto contasPagasDto)
    {
        try
        {
            // Obtendo o Id do usuário autenticado
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _contasPagasService.PagarConta(contasPagasDto, userId);
            return Ok("Conta paga com sucesso!");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize("AuthenticationUser")]
    public IEnumerable<ReadContasPagasDto> GetContasPagas([FromQuery] string? fornecedor = null)
    {
        // Obtendo o Id do usuário autenticado
        string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Inicializa a query para selecionar todas as contas pagas
        var contasPagasQuery = _context.ContasPagas.AsQueryable();

        // Se o parâmetro fornecedor não for nulo ou vazio, adiciona um filtro à query
        if (!string.IsNullOrEmpty(fornecedor))
        {
            // Dividindo a string do fornecedor em palavras
            string[] palavrasFornecedor = fornecedor.Split(' ');

            // Pegando a primeira palavra
            string primeiraPalavraFornecedor = palavrasFornecedor[0];

            // Adiciona um filtro para selecionar apenas as contas pagas que correspondem ao fornecedor especificado
            contasPagasQuery = contasPagasQuery
                .Where(cp => _context.ContasPagarRecorrencia
                    .Any(cpr => cpr.Id == cp.ContasPagarRecorrenciaId && cpr.Fornecedor.StartsWith(primeiraPalavraFornecedor)));
        }

        // Executa a query e obtém a lista de contas pagas
        var contasPagasList = contasPagasQuery.ToList();

        // Mapeia a lista de contas pagas para uma lista de DTOs ReadContasPagasDto
        return _mapper.Map<List<ReadContasPagasDto>>(contasPagasList);
    }


    [HttpGet("ultimasdespesas/semana")]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> GetUltimasDespesasSemana()
    {
        try
        {
            //// Obtendo o ID do usuário autenticado
            //string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtendo as datas de início e fim da semana atual
            DateTime dataAtual = DateTime.Now;
            DateTime inicioSemana = dataAtual.AddDays(-(int)dataAtual.DayOfWeek);
            DateTime fimSemana = inicioSemana.AddDays(6);

            // Consultando as despesas do usuário para a semana atual
            var despesasSemana = await _context.ContasPagas
                // se quiser que mostre as contas pagas somente pelo usuário autenticado, descomentar essa linha
                //.Where(cp => cp.CreatedByUserId == userId && cp.DataPagamento >= inicioSemana && cp.DataPagamento <= fimSemana)

                .Where(cp => cp.DataPagamento >= inicioSemana && cp.DataPagamento <= fimSemana)
                .Include(cp => cp.ContasPagarRecorrencia)
                .ToListAsync();

            // Calculando o valor total para a semana
            decimal totalSemana = despesasSemana.Sum(cp => cp.ContasPagarRecorrencia.Valor);

            // Mapeando para DTOs
            var despesasSemanaDto = _mapper.Map<List<ReadContasPagasDto>>(despesasSemana);

            return Ok(new
            {
                DespesasSemana = despesasSemanaDto,
                TotalSemana = totalSemana
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpGet("ultimasdespesas/mes")]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> GetUltimasDespesasMes()
    {
        try
        {
            // Obtendo o ID do usuário autenticado
            //string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtendo as datas de início e fim do mês atual
            DateTime dataAtual = DateTime.Now;
            DateTime inicioMes = new DateTime(dataAtual.Year, dataAtual.Month, 1);
            DateTime fimMes = inicioMes.AddMonths(1).AddDays(-1);

            // Consultando as despesas do usuário para o mês atual
            var despesasMes = await _context.ContasPagas
                // se quiser que mostre as contas pagas somente pelo usuário autenticado, descomentar essa linha
                //.Where(cp => cp.CreatedByUserId == userId && cp.DataPagamento >= inicioMes && cp.DataPagamento <= fimMes)

                .Where(cp => cp.DataPagamento >= inicioMes && cp.DataPagamento <= fimMes)
                .Include(cp => cp.ContasPagarRecorrencia)
                .ToListAsync();

            // Calculando o valor total para o mês
            decimal totalMes = despesasMes.Sum(cp => cp.ContasPagarRecorrencia.Valor);

            // Mapeando para DTOs
            var despesasMesDto = _mapper.Map<List<ReadContasPagasDto>>(despesasMes);

            return Ok(new
            {
                DespesasMes = despesasMesDto,
                TotalMes = totalMes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpGet("ultimasdespesas/ano")]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> GetUltimasDespesasAno()
    {
        try
        {
            // Obtendo o ID do usuário autenticado
            //string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtendo as datas de início e fim do ano atual
            DateTime dataAtual = DateTime.Now;
            DateTime inicioAno = new DateTime(dataAtual.Year, 1, 1);
            DateTime fimAno = new DateTime(dataAtual.Year, 12, 31);

            // Consultando as despesas do usuário para o ano atual
            var despesasAno = await _context.ContasPagas
                // se quiser que mostre as contas pagas somente pelo usuário autenticado, descomentar essa linha
                //.Where(cp => cp.CreatedByUserId == userId && cp.DataPagamento >= inicioAno && cp.DataPagamento <= fimAno)

                .Where(cp => cp.DataPagamento >= inicioAno && cp.DataPagamento <= fimAno)
                .Include(cp => cp.ContasPagarRecorrencia)
                .ToListAsync();

            // Calculando o valor total para o ano
            decimal totalAno = despesasAno.Sum(cp => cp.ContasPagarRecorrencia.Valor);

            // Mapeando para DTOs
            var despesasAnoDto = _mapper.Map<List<ReadContasPagasDto>>(despesasAno);

            return Ok(new
            {
                DespesasAno = despesasAnoDto,
                TotalAno = totalAno
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }

    [HttpGet("ultimasdespesas")]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> GetUltimasDespesasPagas()
    {
        try
        {
            // Consultando as últimas despesas pagas no geral
            var ultimasDespesasPagas = await _context.ContasPagas
                .Include(cp => cp.ContasPagarRecorrencia)
                .OrderByDescending(cp => cp.DataPagamento)
                .Take(10) // Altere conforme necessário para obter o número desejado de despesas
                .ToListAsync();

            // Mapeando para DTOs
            var despesasPagasDto = _mapper.Map<List<ReadContasPagasDto>>(ultimasDespesasPagas);

            // Obtendo a forma de pagamento de cada despesa
            var formasPagamento = ultimasDespesasPagas.Select(cp => cp.FormaPagamento).Distinct().ToList();

            return Ok(new
            {
                DespesasPagas = despesasPagasDto,
                FormasPagamento = formasPagamento
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro interno do servidor: " + ex.Message);
        }
    }


    [HttpGet("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult GetContasPagasId(int id)
    {
        var contasPagas = _context.ContasPagas.FirstOrDefault(
            contasPagas => contasPagas.Id == id);
        if (contasPagas == null) NotFound();
        var contasDto = _mapper.Map<ReadContasPagasDto>(contasPagas);
        return Ok(contasDto);
    }


    [HttpPut("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult UpdateContasPagas(int id,
        [FromBody] UpdateContasPagasDto updateContasPagasDto)
    {
        var contasPagas = _context.ContasPagas.FirstOrDefault(
            contasPagas => contasPagas.Id == id);
        if (contasPagas == null) NotFound();

        _mapper.Map(updateContasPagasDto, contasPagas);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult DeleteContaPaga(int id)
    {
        var contaPaga = _context.ContasPagas.FirstOrDefault(
            contaPaga => contaPaga.Id == id);
        if (contaPaga == null) NotFound();

        _context.Remove(contaPaga);
        _context.SaveChanges();
        return NoContent();
    }
}