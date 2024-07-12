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
public class CategoriasController : ControllerBase
{
    private readonly EntidadesContext _context;
    private readonly IMapper _mapper;
    private readonly CategoriaService _categoriaService;

    public CategoriasController(EntidadesContext context,
        IMapper mapper, CategoriaService categoriaService)
    {
        _context = context;
        _mapper = mapper;
        _categoriaService = categoriaService;
    }

    [HttpPost]
    [Authorize("AuthenticationUser")]
    public async Task<IActionResult> CriarCategoria([FromBody]
    CreateCategoriaDto categoriaDto)
    {
        try
        {
            // Obtenha o Id do usuário autenticado
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoriaService.CriarCategoria(categoriaDto, userId);
            return Ok("Categoria criada com sucesso");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet]
    [Authorize("AuthenticationUser")]
    public IEnumerable<ReadCategoriasDto> GetCategorias(
       [FromQuery] string? tituloCategoria = null)
    {
        if (tituloCategoria == null)
        {
            return _mapper.Map<List<ReadCategoriasDto>>(
            _context.Categorias.ToList());
        }

        // Dividindo a string do fornecedor em palavras
        string[] palavrasCategoria = tituloCategoria.Split(' ');

        // Pegando a primeira palavra
        string primeiraPalavraCategoria = palavrasCategoria[0];

        // Se o titulo não for nulo
        // Where -> busca o titulo da categoria
        // Any -> Verifica se essa conta tem uma categoria, se sim busca o nome atribuido na variável
        return _mapper.Map<List<ReadCategoriasDto>>(_context.Categorias
            .Where(categoria => categoria.TituloCategoria.StartsWith(primeiraPalavraCategoria))
            //.Any(contas => contas.Categorias.TituloCategoria == tituloContaPagar))
            .ToList());
    }


    [HttpGet("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult GetCategoriaId(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);
        if (categoria == null) return NotFound();
        var categoriaDto = _mapper.Map<ReadCategoriasDto>(categoria);
        return Ok(categoriaDto);
    }

    [HttpPut("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult UpdateCategoria(int id,
        [FromBody] UpdateCategoriasDto updateCategoriasDto)
    {
        var categoria = _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);
        if (categoria == null) return NotFound();

        _mapper.Map(updateCategoriasDto, categoria);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize("AuthenticationUser")]
    public IActionResult DeleteCategoria(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(categoria => categoria.Id == id);
        if (categoria == null) return NotFound();

        _context.Remove(categoria);
        _context.SaveChanges();
        return NoContent();
    }
}
