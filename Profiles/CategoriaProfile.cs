using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Profiles;

public class CategoriaProfile : Profile
{
    public CategoriaProfile()
    {
        CreateMap<CreateCategoriaDto, Categorias>();

        CreateMap<Categorias, ReadCategoriasDto>()
            .ForMember(categoriaDto => categoriaDto.ReadContasPagarDto,
                opt => opt.MapFrom(categoria => categoria.ContasAPagar));

        CreateMap<UpdateCategoriasDto, Categorias>();

    }
}
