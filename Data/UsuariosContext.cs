using ERP_Financeiro_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ERP_Financeiro_API.Data;

public class UsuariosContext : IdentityDbContext<Usuario>
{
    public UsuariosContext(DbContextOptions<UsuariosContext> opts) :
     base(opts)
    { }


    // Resolvendo o erro “Specified key was too long” no Identity Core com MySQL
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tabela "aspnetusers"
        modelBuilder.Entity<Usuario>(entity => {
            entity.Property(m => m.Id).HasMaxLength(110);
            entity.Property(m => m.Email).HasMaxLength(127);
            entity.Property(m => m.NormalizedEmail).HasMaxLength(127);
            entity.Property(m => m.NormalizedUserName).HasMaxLength(127);
            entity.Property(m => m.UserName).HasMaxLength(127);

        });
        modelBuilder.Entity<Usuario>()
               .Property(u => u.Perfil)
               .HasConversion<string>();

        // Tabela "aspnetroles"
        modelBuilder.Entity<IdentityRole>(entity => {
            entity.Property(m => m.Id).HasMaxLength(200);
            entity.Property(m => m.Name).HasMaxLength(127);
            entity.Property(m => m.NormalizedName).HasMaxLength(127);
        });

        // Tabela "aspnetuserlogins"
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.Property(m => m.LoginProvider).HasMaxLength(50);
            entity.Property(m => m.ProviderKey).HasMaxLength(50);
        });

        // Tabela "aspnetuserroles"
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(50);
            entity.Property(m => m.RoleId).HasMaxLength(50);
        });

        // Tabela "aspnetusertokens"
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(m => m.UserId).HasMaxLength(50);
            entity.Property(m => m.LoginProvider).HasMaxLength(50);
            entity.Property(m => m.Name).HasMaxLength(110);

        });
    }
}

