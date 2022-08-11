using AutoMapper;
using GatheredData.Api.AutoMapperProfiles;
using GatheredData.Api.Controllers;
using GatheredData.Api.Models;
using GatheredData.Api.Services;
using Moq;

namespace GatheredData.Test.Controllers;
public class ProductsControllerTest
{

    [Fact]
    public async Task Get_Should_Returns_ProductPayLoadDto_List()
    {
        // Arrange
        var mockedProducts = new List<Product>()
            {
                new()
                {
                    Id = "61a6058e6c43f32854e51f51",
                    ProductName = "iphone 13 pro max"
                },
                new()
                {
                    Id = "61a6058e6c43f32854e51f52",
                    ProductName = "iphone 13"
                }
            };

        var mockRepo = new Mock<IProductsService>();
        mockRepo.Setup(repo => repo.GetAsync())
            .ReturnsAsync(mockedProducts);

        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        IMapper mapper = new Mapper(configuration);


        var controller = new ProductsController(mockRepo.Object, mapper);

        // Act
        var result = await controller.Get();

        // Assert
        Assert.NotNull(result);
    }
}