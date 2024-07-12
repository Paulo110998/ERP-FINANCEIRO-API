using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ERP_Financeiro_API.Authorization;

public class EntidadesAuthorization : AuthorizationHandler<AuthenticationUser>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthenticationUser requirement)
    {
        // Obtém o ID do usuário automaticamente
        var usuarioClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Usando o ID do usuário para verificar a autorização
        if (!string.IsNullOrEmpty(usuarioClaim))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }


    // FindFirst -> Recupera a primeira declaração que corresponde a uma condição especificada.
    // User -> O ClaimsPrincipal que representa o usuário atual
    // ClaimTypes.NameIdentifier -> Representa o tipo de identificador de usuário (ID)
    // IsNullOrEmpty -> indica se a string especificada é nula ou uma string vazia
}