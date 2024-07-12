using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Data;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Services;

public class CategoriaService
{
    private readonly EntidadesContext _entidadesContext;

    public CategoriaService(EntidadesContext entidadesContext)
    {
        _entidadesContext = entidadesContext;
    }

    public async Task CriarCategoria(CreateCategoriaDto categoriaDto, string userId)
    {
        // Verifica se o Id do usuário é válido
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("Não foi possível obter o Id do usuário autenticado");
        }

        // Agora você pode usar o userId para associar à categoria
        Categorias novaCategoria = new Categorias
        {
            TituloCategoria = categoriaDto.TituloCategoria,
            DescricaoCategoria = categoriaDto.DescricaoCategoria,
            CreatedByUserId = userId

        };

        // Adicione a nova categoria ao contexto do banco de dados
        _entidadesContext.Categorias.Add(novaCategoria);

        // Salve as alterações no banco de dados
        await _entidadesContext.SaveChangesAsync();
    }
}