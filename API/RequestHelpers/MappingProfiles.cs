namespace API.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateProductDto, API.Entities.Product>();
        CreateMap<UpdateProductDto, API.Entities.Product>();
    }
}