using AutoMapper;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;
using ERP_Financeiro_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_Financeiro_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ContasPagarRecorrenciaController : ControllerBase
{
    private readonly EntidadesContext _context;
    private IMapper _mapper;
    private readonly ContasPagarRecorrenciaService _contasPagarRecorrenciaService;

    public ContasPagarRecorrenciaController(EntidadesContext context,
        IMapper mapper,
        ContasPagarRecorrenciaService contasPagarRecorrenciaService)
    {
        _context = context;
        _mapper = mapper;
        _contasPagarRecorrenciaService = contasPagarRecorrenciaService;
    }

    [HttpPost]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> CadastrarContasPagarRecorrencia(
        [FromBody] CreateContasPagarRecorrenciaDto contasPagarRecorrenciaDto)
    {
        try
        {
            // Obter o Id do usuário autenticado
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _contasPagarRecorrenciaService.CriarContasPagarRecorrencia(contasPagarRecorrenciaDto, userId);
            return Ok("Conta recorrente cadastrada com sucesso!");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("periodo")]
    public IEnumerable<ReadContasPagarRecorrenciaDto> ObterContasRecorrentePorPeriodo(
    [FromQuery] DateTime? dataInicio = null,
    [FromQuery] DateTime? dataFim = null)
    {
        //IQueryable -> Fornece funcionalidade para avaliar consultas em uma
        //fonte de dados específica em que o tipo de dados não é especificado.
        IQueryable<ContasPagarRecorrencia> query = _context.ContasPagarRecorrencia;

        switch ((dataInicio != null, dataFim != null))
        {
            case (true, true):
                // Caso ambas as datas de início e fim estejam definidas
                query = query.Where(conta =>
                    // Filtra as contas onde a data de início da recorrência é posterior ou igual à data de início fornecida
                    conta.DataInicioRecorrencia >= dataInicio &&
                    // E a data de vencimento é anterior ou igual à data final fornecida
                    conta.DataVencimento <= dataFim); ;
                break;
            case (true, false):
                query = query.Where(conta => conta.DataInicioRecorrencia >= dataInicio);
                break;
            case (false, true):
                query = query.Where(conta => conta.DataVencimento <= dataFim);
                break;
            case (false, false):
                // Nenhum filtro aplicado, retorna todas as contas
                break;
        }

        return _mapper.Map<List<ReadContasPagarRecorrenciaDto>>(query.ToList());
    }

    [HttpGet]
    [Authorize("AuthenticationUser")]
    public IEnumerable<ReadContasPagarRecorrenciaDto> GetContasPagarRecorrencia(
        [FromQuery] string? fornecedor = null)
    {
        if (fornecedor == null)
        {
            return _mapper.Map<List<ReadContasPagarRecorrenciaDto>>(
            _context.ContasPagarRecorrencia.ToList());
        }

        // Dividindo a string do fornecedor em palavras
        string[] palavrasFornecedor = fornecedor.Split(' ');

        // Pegando a primeira palavra
        string primeiraPalavraFornecedor = palavrasFornecedor[0];

        return _mapper.Map<List<ReadContasPagarRecorrenciaDto>>(_context.ContasPagarRecorrencia
            .Where(f => f.Fornecedor.StartsWith(primeiraPalavraFornecedor))
            .ToList());
    }


    [HttpGet("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult GetContasPagarRecorrenciaId(int id)
    {
        var contas = _context.ContasPagarRecorrencia.FirstOrDefault(
            contas => contas.Id == id);
        if (contas == null) NotFound();
        var contasDto = _mapper.Map<ReadContasPagarRecorrenciaDto>(contas);
        return Ok(contasDto);
    }

    [HttpPut("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult UpdateContasParagarRecorrenciaId(int id,
        [FromBody] UpdateContasPagarRecorrenciaDto updateContasPagarRecorrenciaDto)
    {
        var contas = _context.ContasPagarRecorrencia.FirstOrDefault(
            contas => contas.Id == id);
        if (contas == null) NotFound();

        _mapper.Map(updateContasPagarRecorrenciaDto, contas);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult DeleteContasPagarRecorrente(int id)
    {
        var contas = _context.ContasPagarRecorrencia.FirstOrDefault(
            contas => contas.Id == id);
        if (contas == null) NotFound();

        _context.Remove(contas);
        _context.SaveChanges();
        return NoContent();
    }
}
