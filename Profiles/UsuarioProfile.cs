using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<CreateUsuarioDto, Usuario>();

        CreateMap<Usuario, ReadUsuariosDto>()
            .ForMember(dest => dest.Perfil, opt => opt.MapFrom(src => src.Perfil.ToString()));

        CreateMap<UpdateUsuarioDto, Usuario>();
    }
}
