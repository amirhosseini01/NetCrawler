using AutoMapper;
using GatheredData.Api.Dtos;
using GatheredData.Api.Models;

namespace GatheredData.Api.AutoMapperProfiles;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
         //Finance
        CreateMap<Product, ProductInputDto>();
        CreateMap<ProductInputDto, Product>();
        CreateMap<Product, ProductPayLoadDto>();
    }
}