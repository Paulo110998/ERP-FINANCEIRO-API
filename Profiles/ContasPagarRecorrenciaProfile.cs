using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Profiles;

public class ContasPagarRecorrenciaProfile : Profile
{
    public ContasPagarRecorrenciaProfile()
    {
        CreateMap<CreateContasPagarRecorrenciaDto, ContasPagarRecorrencia>();

        CreateMap<ContasPagarRecorrencia, ReadContasPagarRecorrenciaDto>()
            .ForMember(contaDto => contaDto.ReadContasPagas,
            opt => opt.MapFrom(conta => conta.ContasPagas));

        CreateMap<UpdateContasPagarRecorrenciaDto, ContasPagarRecorrencia>();
    }
}
