using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Profiles;

public class BeneficiarioProfile : Profile
{
    public BeneficiarioProfile()
    {
        CreateMap<CreateBeneficiariosDto, Beneficiarios>();

        CreateMap<Beneficiarios, ReadBeneficiariosDto>()
            .ForMember(beneficiarioDto => beneficiarioDto.ReadContasPagarRecorrenciaDto,
            opt => opt.MapFrom(beneficiario => beneficiario.ContasPagarRecorrencia));

        CreateMap<UpdateBeneficiarioDto, Beneficiarios>();
    }
}
