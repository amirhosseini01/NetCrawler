using AutoMapper;
using GatheredData.Api.AutoMapperProfiles;
using GatheredData.Api.Controllers;
using GatheredData.Api.Dtos;
using GatheredData.Api.Models;
using GatheredData.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GatheredData.Test.Controllers;
public class ProductsControllerTest
{
    private readonly Mock<IProductsService> _mockRepo;
    private readonly IMapper _mapper;
    public ProductsControllerTest()
    {
        _mockRepo = new Mock<IProductsService>();

        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        _mapper = new Mapper(configuration);
    }

    [Fact]
    public async Task Get_Should_Returns_ProductPayLoadDto_List()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAsync())
             .ReturnsAsync(new List<Product>()
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
             });

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Get();

        // Assert

        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.IsType<ActionResult<List<ProductPayLoadDto>>>(result);
        Assert.IsAssignableFrom<List<ProductPayLoadDto>>(result.Value);
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f53")]
    public async Task Get_Should_Returns_ProductPayLoadDto_ById(string id)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAsync(id))
             .ReturnsAsync(new Product()
             {
                 Id = "61a6058e6c43f32854e51f53",
                 ProductName = "iphone 13 pro max"
             });

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Get(id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.IsType<ActionResult<ProductPayLoadDto>>(result);
        Assert.IsAssignableFrom<ProductPayLoadDto>(result.Value);
        Assert.Equal("61a6058e6c43f32854e51f53", result.Value?.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData(" ")]
    public async Task Get_Should_Returns_BadRequest_When_Id_IsNotValid(string id)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAsync(id));

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Get(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductPayLoadDto>>(result);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f53")]
    public async Task Get_Should_Returns_NotFound_When_NoItem_Founded(string id)
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAsync(id))
            .ReturnsAsync(() => null);

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Get(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductPayLoadDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
}