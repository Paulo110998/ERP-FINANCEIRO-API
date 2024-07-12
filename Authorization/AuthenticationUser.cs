using Microsoft.AspNetCore.Authorization;

namespace ERP_Financeiro_API.Authorization;

public class AuthenticationUser : IAuthorizationRequirement
{
    public AuthenticationUser()
    {

    }
}
