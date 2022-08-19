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

    #region Get (id)
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
    [InlineData("1234567812345678123456789")]
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

        _mockRepo.Setup(repo => repo.GetAsync(id))
            .ReturnsAsync(() => null);

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Get(id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductPayLoadDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    #endregion

    #region Post
    [Fact]
    public async Task Post_Should_Returns_CreatedAtRouteResult_WhenItem_Added_Successfully()
    {
        // Arrange
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Post(dto);

        // Assert

        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(actionResult.Value);
        Assert.IsType<ProductPayLoadDto>(actionResult.Value);

        ProductPayLoadDto returnedProductPayLoadDto = (ProductPayLoadDto)(actionResult.Value ??
            throw new ArgumentException(nameof(actionResult.Value)));

        Assert.Equal(nameof(controller.Get), actionResult.ActionName);
        Assert.NotNull(actionResult.RouteValues);
        Assert.True(actionResult.RouteValues?.ContainsKey("id"));

        object? routVal = null;
        Assert.True(actionResult.RouteValues?.TryGetValue("id", out routVal));
        Assert.Equal(routVal?.ToString(), dto.Id);

        ProductPayLoadDto expectedReturnedDto = new()
        {
            Id = dto.Id,
            ProductName = dto.ProductName ?? string.Empty
        };

        Assert.Equal(expectedReturnedDto.Id, returnedProductPayLoadDto.Id);
        Assert.Equal(expectedReturnedDto.ProductName, returnedProductPayLoadDto.ProductName);
    }

    [Fact]
    public async Task Post_Should_Returns_BadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");

        var controller = new ProductsController(_mockRepo.Object, _mapper);
        controller.ModelState.AddModelError(nameof(dto.Id), "Required");

        // Act
        var result = await controller.Post(dto);

        // Assert

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Post_Should_Returns_BadRequest_WhenAnItem_AlreadyExist()
    {
        // Arrange
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");
        _mockRepo.Setup(repo => repo.AnyAsync(dto.Id!))
            .ReturnsAsync(() => true);

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Post(dto);

        // Assert

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }
    #endregion

    #region Update
    [Fact]
    public async Task Update_Should_Returns_NoContentResult_WhenItem_Updated_Successfully()
    {
        // Arrange
        string id = "61a6058e6c43f32854e51f53";
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");

        _mockRepo.Setup(repo => repo.GetAsync(id))
             .ReturnsAsync(new Product()
             {
                 Id = id,
                 ProductName = "iphone 13 pro max"
             });

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Update(id, dto);

        // Assert

        var actionResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_Should_Returns_BadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        string id = "61a6058e6c43f32854e51f53";
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");

        var controller = new ProductsController(_mockRepo.Object, _mapper);
        controller.ModelState.AddModelError(nameof(dto.Id), "Required");

        // Act
        var result = await controller.Update(id, dto);

        // Assert

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Update_Should_Returns_NotFoundResult_WhenAnItem_NotFounded()
    {
        // Arrange
        string id = "61a6058e6c43f32854e51f53";
        ProductInputDto dto = new(Id: "61a6058e6c43f32854e51f53", ProductName: "iphone 13 pro max");
        _mockRepo.Setup(repo => repo.GetAsync(id))
            .ReturnsAsync(() => null);

        var controller = new ProductsController(_mockRepo.Object, _mapper);

        // Act
        var result = await controller.Update(id, dto);

        // Assert

        var badRequestResult = Assert.IsType<NotFoundResult>(result);
    }
    #endregion

    #region Delete
    [Theory]
    [InlineData("61a6058e6c43f32854e51f53")]
    public async Task Delete_Should_Returns_NoContentResult_When_Item_Removed_Successfully(string id)
    {
        //Arrage
        _mockRepo.Setup(x => x.AnyAsync(id))
            .ReturnsAsync(true);

        ProductsController controller = new ProductsController(_mockRepo.Object, _mapper);

        //Act
        var result = await controller.Delete(id);

        //Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("1234567812345678123456789")]
    [InlineData(" ")]
    public async Task Delete_Sould_Returns_BadRequestResult_When_Id_Invalid(string id)
    {
        //Arrage
        ProductsController productsController = new ProductsController(_mockRepo.Object, _mapper);

        //Act
        var result = await productsController.Delete(id);

        //Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f53")]
    public async Task Delete_Sould_Returns_NotFoundResult_When_NoItem_Founded(string id)
    {
        //Arrage
        _mockRepo.Setup(x => x.AnyAsync(id))
            .ReturnsAsync(() => false);
        ProductsController productsController = new ProductsController(_mockRepo.Object, _mapper);

        //Act
        var result = await productsController.Delete(id);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }
    #endregion

    #region IsValidMongoDBId
    [Theory]
    [InlineData("61a6058e6c43f32854e51f53")]
    public void IsValidMongoDBId_Should_Returns_True_When_Id_Is_Valid(string id)
    {
        //Arrange

        //Act
        ProductsController productsController = new ProductsController(_mockRepo.Object, _mapper);

        //Assert
        var result = productsController.IsValidMongoDBId(id);

        Assert.True(result);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234")]
    [InlineData("1234567812345678123456789")]
    public void IsValidMongoDBId_Should_Returns_False_When_Id_Is_NotValid(string id)
    {
        //Arrange

        //Act
        ProductsController productsController = new ProductsController(_mockRepo.Object, _mapper);

        //Assert
        var result = productsController.IsValidMongoDBId(id);

        Assert.False(result);
    }
    #endregion
}