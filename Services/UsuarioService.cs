using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;
using Microsoft.AspNetCore.Identity;

namespace ERP_Financeiro_API.Services;

public class UsuarioService
{
    private const int MAX_IMAGE_SIZE_BYTES = 3145728; // 3 MB - Tamanho máximo da imagem 
    private readonly IMapper _mapper;

    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;
    private readonly UsuariosContext _context;

    public UsuarioService(IMapper mapper,
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        RoleManager<IdentityRole> roleManager,
        TokenService tokenService,
        EmailService emailService,
        UsuariosContext usuariosContext)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _context = usuariosContext;
    }


    // MÉTODO PARA CADASTRAR UM USUÁRIO SEM LOGIN
    public async Task RegisterUser(CreateUsuarioDto createUsuarioDto)
    {
        // Verificar o tamanho da imagem
        if (createUsuarioDto.ImageProfile != null && createUsuarioDto.ImageProfile.Length > MAX_IMAGE_SIZE_BYTES)
        {
            throw new ApplicationException("Tamanho da imagem excede o limite permitido.");
        }

        // Mapeando a classe de usuário
        Usuario user = _mapper.Map<Usuario>(createUsuarioDto);

        // Cadastrando um usuário no banco
        IdentityResult result = await _userManager.CreateAsync(user, createUsuarioDto.Password);

        // Se o resultado tiver sucesso
        if (result.Succeeded)
        {
            // Atribui o perfil ao novo usuário (default = Assistente)
            switch (user.Perfil)
            {
                case Perfil.Assistente:
                    await AtribuirPerfilAssistente(user.Id);
                    break;
            }

            // Envie o e-mail de boas-vindas
            await _emailService.PasswordResetEmail(createUsuarioDto.Email, createUsuarioDto.Username);
        }
        else
        {
            // Capturar e lançar um erro mais detalhado
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Erro ao cadastrar usuário: {errors}");
        }
    }

    // MÉTODO PARA CONFERIR SE O ROLE EXISTE
    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // MÉTODO PARA ATRIBUIR A FUNÇÃO DE ADMIN
    public async Task<bool> AtribuirPerfilAdmin(string userId)
    {
        // Localiza e retorna um usuário, se houver, que tenha o userId
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false; // Usuário não encontrado
        }

        // Remove o usuário do papel "Admin", se já estiver nele, para evitar a mensagem "User is already in role Admin"
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        // Define o perfil e role do usuário
        user.Perfil = Perfil.Admin;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return false;
        }

        // Se o role existir 
        await EnsureRoleExistsAsync("Admin");
        // Adiciona o usuário ao role
        var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
        return roleResult.Succeeded;
    }

    // MÉTODO PARA ATRIBUIR A FUNÇÃO DE ASSISTENTE
    public async Task<bool> AtribuirPerfilAssistente(string userId)
    {
        // Localiza e retorna um usuário, se houver, que tenha o userId
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // Remove o usuário do papel "Assistente", se já estiver nele para evitar a mensagem "User is already in role Assistente"
        if (await _userManager.IsInRoleAsync(user, "Assistente"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Assistente");
        }

        // Define o perfil e role do usuário
        user.Perfil = Perfil.Assistente;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return false;
        }

        // Se o role existir 
        await EnsureRoleExistsAsync("Assistente");
        // Adiciona o usuário ao role
        var roleResult = await _userManager.AddToRoleAsync(user, "Assistente");
        return roleResult.Succeeded;
    }


    // MÉTODO PARA LOGIN
    public async Task<string> LoginUser(LoginUsuarioDto loginUsuarioDto)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(loginUsuarioDto.Email);

        if (user == null)
        {
            throw new ApplicationException("Erro ao acessar sua conta, verifique seus dados de email e senha.");
        }

        // Verificando se o usuário é válido antes de gerar o token
        if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
        {
            throw new ApplicationException("A conta está bloqueada.");
        }

        // Verificando a senha do usuário
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginUsuarioDto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            throw new ApplicationException("Erro ao acessar sua conta, verifique seus dados de email e senha.");
        }

        // Gerando o token apenas se o usuário for autenticado com sucesso
        var token = _tokenService.GenerateToken(user);

        return token;
    }


    // MÉTODO PARA ENVIAR EMAIL DE RESETAR SENHA
    public async Task<string> PasswordReset(string email)
    {
        // verificando email do usuário
        var user = await _userManager.FindByEmailAsync(email.ToLower());

        // se o email do user for nulo/não existir
        if (user == null)
        {
            throw new ApplicationException("Usuário não encontrado para o email fornecido.");
        }

        // gerando um token para reset de senha
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        Console.WriteLine(token); // Adicione este log para verificar o token gerado


        await _emailService.PasswordResetEmail(email, token);

        return token;
    }


    // MÉTODO PARA ALTERAR A SENHA
    public async Task ChangePassword(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email.ToLower());

        if (user == null)
        {
            throw new ApplicationException("Usuário não encontrado");
        }

        // Verifica se o token é válido antes de redefinir a senha
        var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, UserManager<Usuario>.ResetPasswordTokenPurpose, token);

        if (!isTokenValid)
        {
            throw new ApplicationException("Token inválido");
        }

        // Agora podemos redefinir a senha
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            throw new ApplicationException("Erro ao redefinir a senha");
        }
    }
}
