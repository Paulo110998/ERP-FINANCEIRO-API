using AutoMapper;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_Financeiro_API.Controllers;


[ApiController]
[Route("[controller]")]
public class BeneficiariosController : ControllerBase
{
    private readonly EntidadesContext _context;
    private readonly IMapper _mapper;
    private readonly BeneficiariosService _beneficiariosService;

    public BeneficiariosController(EntidadesContext context,
        IMapper mapper, BeneficiariosService beneficiariosService)
    {
        _context = context;
        _mapper = mapper;
        _beneficiariosService = beneficiariosService;
    }


    [HttpPost]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> CadastrarBeneficiario([FromBody]
    CreateBeneficiariosDto beneficiariosDto)
    {
        try
        {
            //Obtendo o Id do usuário autenticado
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _beneficiariosService.CriarBeneficiario(beneficiariosDto, userId);
            return Ok("Beneficiário cadastrado com sucesso!");

        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize("AuthenticationUser")]
    public IEnumerable<ReadBeneficiariosDto> GetBeneficiarios(
        [FromQuery] string? NomeBeneficiarios = null)
    {
        if (NomeBeneficiarios == null)
        {
            return _mapper.Map<List<ReadBeneficiariosDto>>(
                _context.Beneficiarios.ToList());
        }

        // Dividindo a string do fornecedor em palavras
        string[] palavrasBeneficiario = NomeBeneficiarios.Split(' ');

        // Pegando a primeira palavra
        string primeiraPalavraBeneficiario = palavrasBeneficiario[0];

        // Se o titulo não for nulo
        // Where -> busca o nome do beneficiário
        return _mapper.Map<List<ReadBeneficiariosDto>>(_context.Beneficiarios
            .Where(beneficiarios => beneficiarios.NomeBeneficiario.StartsWith(primeiraPalavraBeneficiario))
            .ToList());
    }

    [HttpGet("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult GetBeneficiariosId(int id)
    {
        var beneficiario = _context.Beneficiarios.FirstOrDefault(beneficiario => beneficiario.Id == id);
        if (beneficiario == null) return NotFound();
        var beneficiarioDto = _mapper.Map<ReadBeneficiariosDto>(beneficiario);
        return Ok(beneficiarioDto);
    }

    [HttpPut("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult UpdateBeneficiario(int id,
        [FromBody] UpdateBeneficiarioDto updateBeneficiarioDto)
    {
        var beneficiario = _context.Beneficiarios.FirstOrDefault(beneficiario => beneficiario.Id == id);
        if (beneficiario == null) return Ok();

        _mapper.Map(updateBeneficiarioDto, beneficiario);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult DeleteBeneficiario(int id)
    {
        var beneficiario = _context.Beneficiarios.FirstOrDefault(
            beneficiario => beneficiario.Id == id);

        _context.Remove(beneficiario);
        _context.SaveChanges();
        return NoContent();
    }

}