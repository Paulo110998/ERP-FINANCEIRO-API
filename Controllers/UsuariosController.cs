using AutoMapper;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;
using ERP_Financeiro_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERP_Financeiro_API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuariosController : ControllerBase
{
    private UsuarioService _usuarioService;
    private readonly UserManager<Usuario> _userManager;
    private IMapper _mapper;
    private UsuariosContext _context;


    public UsuariosController(UsuarioService usuarioService,
        UserManager<Usuario> userManager,
        IMapper mapper, UsuariosContext context)
    {
        _usuarioService = usuarioService;
        _userManager = userManager;
        _mapper = mapper;
        _context = context;
    }


    // MÉTODO PARA CADASTRAR UM USUÁRIO COMUM
    [HttpPost("cadastro")]
    public async Task<IActionResult> CadastroAsync(CreateUsuarioDto cadastroDto)
    {
        try
        {
            await _usuarioService.RegisterUser(cadastroDto);
            return Ok("Usuário cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro 400.. verifique seus dados..{ex}");
        }

    }

    // MÉTODO PARA O ADMIN CADASTRAR UM USUÁRIO COMUM
    [HttpPost("cadastro/admin")]
    [Authorize("AuthenticationAdmin")]
    public async Task<IActionResult> CadastroAdminAsync(CreateUsuarioDto createUsuario)
    {
        try
        {
            await _usuarioService.RegisterUser(createUsuario);

            // Atribui o perfil de assistente ao novo usuário criado
            var user = await _userManager.FindByEmailAsync(createUsuario.Email);
            if (user != null)
            {
                await _usuarioService.AtribuirPerfilAssistente(user.Id);
            }

            return Ok("Usuário cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            return Unauthorized($"Você não tem cadastro de admin: {ex}");
        }
    }



    // MÉTODO PARA LOGIN DE USUÁRIO
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginUsuarioDto loginDto)
    {
        try
        {
            var token = await _usuarioService.LoginUser(loginDto);

            if (string.IsNullOrEmpty(token))
            {
                Unauthorized("Token nulo ou inválido..");
            }

            return Ok(token);

        }
        catch (Exception ex)
        {
            return Unauthorized($"Acesso não autorizado, seus dados de acesso..{ex}");
        }
    }


    // MÉTODO PARA O ADMIN ATRIBUIR A FUNÇÃO DE ADMIN
    [HttpPut("atribuir-admin/{userId}")]
    [Authorize("AuthenticationAdmin")]
    public async Task<IActionResult> AtribuirAdmin(string userId)
    {
        try
        {
            // Obtenha o ID do usuário autenticado
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtenha os dados do usuário do banco de dados usando o ID
            var usuario = _context.Users.FirstOrDefault(u => u.Id == user);

            if (usuario == null)
            {
                return NotFound();
            }

            var sucesso = await _usuarioService.AtribuirPerfilAdmin(userId);
            if (sucesso)
            {
                return Ok("Perfil de administrador atribuído com sucesso.");
            }
            else
            {
                return NotFound("Usuário não encontrado.");
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

    }


    // MÉTODO PARA O ADMIN ATRIBUIR A FUNÇÃO DE ASSISTENTE
    [HttpPut("atribuir-assistente/{userId}")]
    [Authorize("AuthenticationAdmin")]
    public async Task<IActionResult> AtribuirAssistente(string userId)
    {

        try
        {
            // Obtenha o ID do usuário autenticado
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtenha os dados do usuário do banco de dados usando o ID
            var usuario = _context.Users.FirstOrDefault(u => u.Id == user);

            if (usuario == null)
            {
                return NotFound();
            }

            var sucesso = await _usuarioService.AtribuirPerfilAssistente(userId);
            if (sucesso)
            {
                return Ok("Perfil de assistente atribuído com sucesso.");
            }
            else
            {
                return NotFound("Usuário não encontrado.");
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

    }



    // MÉTODO PARA ENVIAR O RESET DE SENHA
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(string email)
    {
        try
        {
            email = email.ToLower();
            await _usuarioService.PasswordReset(email);
            return Ok("Email de recuperação de senha enviado com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro 400.. verifique se seu email foi digitado corretamente..{ex}");
        }
    }


    // MÉTODO PARA ALTERAR A SENHA
    [HttpPost("reset-password-confirm")]
    public async Task<IActionResult> ResetPasswordConfirmAsync(ResetPasswordDto resetPasswordDto)
    {
        await _usuarioService.ChangePassword(resetPasswordDto.Email.ToLower(),
           resetPasswordDto.Token, resetPasswordDto.NewPassword);
        _context.SaveChanges();
        return Ok("Senha alterada com sucesso!");
    }


    // MÉTODO PARA BUSCAR A LISTA DE USUÁRIOS
    [HttpGet]
    [Authorize("AuthenticationAdmin")]
    public IEnumerable<ReadUsuariosDto> GetUsuarios(
        [FromQuery] string? username = null)
    {
        if (username == null)
        {
            return _mapper.Map<List<ReadUsuariosDto>>(
          _context.Users.ToList());
        }

        string[] palavrasUsername = username.Split(' ');

        string primeiraPalavraUsername = palavrasUsername[0];

        return _mapper.Map<List<ReadUsuariosDto>>(_context.Users
            .Where(usuario => usuario.UserName.StartsWith(primeiraPalavraUsername))
            .ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetUsuariosId(string id)
    {
        var usuario = _context.Users.FirstOrDefault(user =>
        user.Id == id);
        if (usuario == null) return NotFound();
        var usuariosDto = _mapper.Map<ReadUsuariosDto>(usuario);
        return Ok(usuariosDto);
    }

    [HttpGet("perfil")]
    [Authorize("AuthenticationUser")]
    public IActionResult GetPerfilUsuario()
    {
        // Obtenha o ID do usuário autenticado
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Obtenha os dados do usuário do banco de dados usando o ID
        var usuario = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (usuario == null)
        {
            return NotFound();
        }

        // Mapeie os dados do usuário para o DTO de leitura
        var usuarioDto = _mapper.Map<ReadUsuariosDto>(usuario);

        return Ok(usuarioDto);
    }


    [HttpPut("perfil")]
    [Authorize("AuthenticationUser")]
    public IActionResult UpdateProfile([FromBody] UpdateUsuarioDto updateUsuarioDto)
    {
        // Obtenha o ID do usuário autenticado
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Obtenha o usuário do banco de dados usando o ID
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return NotFound();

        _mapper.Map(updateUsuarioDto, user);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut("{id}")]
    [Authorize("AuthenticationAdmin")]
    public IActionResult UpdateCadastro(string id,
        [FromBody] UpdateUsuarioDto updateUsuarioDto)
    {
        var user = _context.Users.FirstOrDefault(
            user => user.Id == id);
        if (user == null) NotFound();

        _mapper.Map(updateUsuarioDto, user);
        _context.SaveChanges();
        return NoContent();
    }


    [HttpDelete("{id}")]
    [Authorize("AuthenticationAdmin")]
    public IActionResult DeleteUsuario(string id)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if (user == null) return NotFound();

        _context.Remove(user);
        _context.SaveChanges();
        return NoContent();
    }

}