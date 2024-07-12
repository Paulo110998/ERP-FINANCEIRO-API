namespace ERP_Financeiro_API.Data.Dto;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}

