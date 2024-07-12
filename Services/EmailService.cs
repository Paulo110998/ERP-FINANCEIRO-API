using System.Net.Mail;
using System.Net;

namespace ERP_Financeiro_API.Services;

public class EmailService
{
    private readonly string _smtpServer = "mail.nextopinion.com.pt";
    private readonly int _smtpPort = 587; // Porta do servidor SMTP
    private readonly string _smtpUsername = "dev@nextopinion.com.pt";
    private readonly string _smtpPassword = "@Next2024";

    // ENVIO DE EMAIL "BEM-VINDO" !
    public async Task WelcomeEmail(string destinatario, string nomeUsuario)
    {
        // Configurando SMPT
        var smtpClient = new SmtpClient(_smtpServer)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = true,
        };

        // Mensagem do corpo do email
        var mensagem = new MailMessage
        {
            From = new MailAddress(_smtpUsername),
            Subject = "Bem-vindo ao ERP - API.",
            Body = $"Olá {nomeUsuario},\n\nBem-vindo ao ERP API! Agradecemos por se cadastrar!",
            IsBodyHtml = false,
        };

        // enviando email para o destinatário
        mensagem.To.Add(destinatario);

        await smtpClient.SendMailAsync(mensagem);
    }

    // ENVIO DE EMAIL PARA RESETAR A SENHA
    public async Task PasswordResetEmail(string destinatario, string token)
    {
        // Configurando SMPT
        var smtpClient = new SmtpClient(_smtpServer)
        {
            Port = _smtpPort,
            Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
            EnableSsl = false,
        };

        // Link para acesso de reset por token
        var resetUrl = $"https://nextcash.com.br/resetar-senha/{WebUtility.UrlEncode(token)}";


        // Mensagem do corpo do email
        var mensagem = new MailMessage
        {
            From = new MailAddress(_smtpUsername),
            Subject = "Recuperação de Senha - NextCash",
            Body = $"Olá,\n\nVocê solicitou a recuperação de senha para a sua conta no NextCash. Clique no link a seguir para redefinir sua senha:\n{resetUrl}\n\nSe você não solicitou esta recuperação, ignore este e-mail.",
            IsBodyHtml = false,
        };

        // enviando email para o destinatário
        mensagem.To.Add(destinatario);

        await smtpClient.SendMailAsync(mensagem);


    }
}