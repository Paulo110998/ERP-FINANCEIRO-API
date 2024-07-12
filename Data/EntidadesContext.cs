using ERP_Financeiro_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ERP_Financeiro_API.Data;

public class EntidadesContext : DbContext
{
    public EntidadesContext(DbContextOptions<EntidadesContext> opts)
    : base(opts)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContasPagarRecorrencia>()
            .Property(e => e.TipoRecorrencia)
            .HasConversion<string>();

        modelBuilder.Entity<Beneficiarios>()
           .Property(e => e.Tipo)
        .HasConversion<string>();
    }


    public DbSet<Beneficiarios> Beneficiarios { get; set; }

    public DbSet<Categorias> Categorias { get; set; }

    public DbSet<ContasPagas> ContasPagas { get; set; }

    public DbSet<ContasPagarRecorrencia> ContasPagarRecorrencia { get; set; }

   

}