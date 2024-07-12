using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERP_Financeiro_API.Authorization;

public class EntidadesAuthorizationAdmin : AuthorizationHandler<AuthenticationAdmin>
{
    private readonly UsuariosContext _usuariosContext;

    public EntidadesAuthorizationAdmin(UsuariosContext usuariosContext)
    {
        _usuariosContext = usuariosContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthenticationAdmin requirement)
    {
        // Obtém o ID do usuário automaticamente
        var usuarioClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Verifica se o usuário tem o perfil de administrador
        var isAdmin = await _usuariosContext.Users.AnyAsync(u => u.Id == usuarioClaim && u.Perfil == Perfil.Admin);

        if (isAdmin)
        {
            context.Succeed(requirement);
        }

    }
}