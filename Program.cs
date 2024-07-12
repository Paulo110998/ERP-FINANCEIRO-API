using ERP_Financeiro_API.Authorization;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;
using ERP_Financeiro_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(); // Adicionar logging para o console

builder.Services.AddMemoryCache();


QuestPDF.Settings.License = LicenseType.Community;

// Buscando conex�o com database para as tabelas das entidades
//var entidadesConnectionString = builder.Configuration["ConnectionStrings:EntidadesConnection"];
var entidadesConnectionString = builder.Configuration.GetConnectionString("EntidadesConnection");

builder.Services.AddDbContext<EntidadesContext>(opt =>
    opt.UseLazyLoadingProxies().UseMySql(entidadesConnectionString, ServerVersion.AutoDetect(entidadesConnectionString)));

// Buscando conex�o com database para as tabelas de usu�rios
//var usersConnectionString = builder.Configuration["ConnectionStrings:UsuariosConnection"];
var usersConnectionString = builder.Configuration.GetConnectionString("UsuariosConnection");

builder.Services.AddDbContext<UsuariosContext>(opt =>
{
    opt.UseMySql(usersConnectionString, ServerVersion.AutoDetect(usersConnectionString));
});

builder.Services
    .AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<UsuariosContext>()
    .AddDefaultTokenProviders();


// M�todo para criar roles
async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Assistente" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Criar a role e adicionar ao banco de dados
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Adicionando o services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<BeneficiariosService>();
builder.Services.AddScoped<ContasPagasService>();
builder.Services.AddScoped<ContasPagarRecorrenciaService>();


// Configura��o de senha 
builder.Services.Configure<IdentityOptions>(options =>
{
    // Define o comprimento m�nimo necess�rio para a senha
    options.Password.RequiredLength = 10;

    // Exige que a senha contenha pelo menos um d�gito num�rico (0-9)
    options.Password.RequireDigit = true;

    // Exige que a senha contenha pelo menos uma letra min�scula (a-z)
    options.Password.RequireLowercase = true;

    // Define que n�o � necess�rio que a senha contenha caracteres n�o alfanum�ricos (como @, #, $)
    options.Password.RequireNonAlphanumeric = false;

    // Exige que a senha contenha pelo menos uma letra mai�scula (A-Z)
    options.Password.RequireUppercase = true;

    // Especifica o n�mero m�nimo de caracteres �nicos que a senha deve conter
    options.Password.RequiredUniqueChars = 1;
});


//// Adicionando servi�o de autentica��o por JWT 
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme =
    JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new
    Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey_ApiFinanceiro"])),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };

});


//Adicionando cookie de redirecionamento..
builder.Services.ConfigureApplicationCookie(options => {
    options.Events.OnRedirectToAccessDenied = context => {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});


// Adicionando autoriza��es de acesso aos dados
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AuthenticationUser", policy =>
    {
        policy.AddRequirements(new AuthenticationUser());
    });

    opts.AddPolicy("AuthenticationAdmin", policy =>
    {
        policy.AddRequirements(new AuthenticationAdmin());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, EntidadesAuthorization>();
builder.Services.AddScoped<IAuthorizationHandler, EntidadesAuthorizationAdmin>();



// Adicionando o mapeamento de DTOs (AutoMapper)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor();

// Configurando prote��o de dados
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/ExternalDataProtectionKeys2"))
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ADICIONANDO REGISTRO DE LOGS //
builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Adicionar provedor de log para console
    logging.AddDebug(); // Adicionar provedor de log para debug
});


// Adiciona a pol�tica de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
            //.AllowCredentials(); // Adicione essa linha para permitir credenciais
        });
});

var app = builder.Build();

// Usa a pol�tica de CORS
app.UseCors("CorsPolicy");

// Ambiente de produ��o
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Chamar o m�todo para criar roles ao iniciar a aplica��o
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
