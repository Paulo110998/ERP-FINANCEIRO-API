using AutoMapper;
using ERP_Financeiro_API.Data.Dto;
using ERP_Financeiro_API.Models;

namespace ERP_Financeiro_API.Profiles;

public class ContasPagasProfile : Profile
{
    public ContasPagasProfile()
    {
        CreateMap<CreateContasPagasDto, ContasPagas>();

        CreateMap<ContasPagas, ReadContasPagasDto>();

        CreateMap<UpdateContasPagasDto, ContasPagas>();
    }
}
