using Microsoft.AspNetCore.Authorization;

namespace ERP_Financeiro_API.Authorization;

public class AuthenticationAdmin : IAuthorizationRequirement
{
    public AuthenticationAdmin()
    {

    }
}
