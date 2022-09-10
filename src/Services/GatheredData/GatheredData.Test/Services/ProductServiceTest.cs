using GatheredData.Api.Models;
using GatheredData.Api.Services;
using Microsoft.Extensions.Options;

namespace GatheredData.Test.Services;
public class ProductServiceTest
{
    private readonly IProductsService _productsService;

    public ProductServiceTest()
    {
        ProductStoreDatabaseSettings productStoreDatabaseSettings = new()
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "NetCrawler",
            ProductsCollectionName = "Products"
        };
        _productsService = new
            ProductsService(Options.Create(productStoreDatabaseSettings));
    }

    [Fact]
    public async Task GetAsync_Should_Return_ListOfProducts()
    {
        //act
        var result = await _productsService.GetAsync();

        //assert
        Assert.NotNull(result);
        Assert.IsType<List<Product>>(result);
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f01")]
    [InlineData("61a6058e6c43f32854e51f02")]
    public async Task GetAsync_Should_Return_Product(string id)
    {
        //arrange
        await _productsService.CreateTestProduct(productId: id);

        //act
        var result = await _productsService.GetAsync(id);

        //assert
        Assert.NotNull(result);
        Assert.IsType<Product>(result);

        //arrange
        await _productsService.RemoveTestProduct(productId: id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("61a6058e6c43f32854e51f00")]
    public async Task GetAsync_Should_Return_Null(string id)
    {
        //act
        var result = await _productsService.GetAsync(id);

        //assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("1234")]
    [InlineData("61a6058e6c43f32854e51f01234")]
    public async Task GetAsync_Should_Throw(string id)
    {
        //act & assert
        await Assert.ThrowsAnyAsync<FormatException>(() => _productsService.GetAsync(id));
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f01")]
    [InlineData("61a6058e6c43f32854e51f02")]
    public async Task AnyAsync_Should_Return_True(string id)
    {
        //arrange
        await _productsService.CreateTestProduct(productId: id);

        //act
        var result = await _productsService.AnyAsync(id);

        //assert
        Assert.True(result);

        //arrange
        await _productsService.RemoveTestProduct(productId: id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("61a6058e6c43f32854e51f00")]
    public async Task AnyAsync_Should_Return_False(string id)
    {
        //act
        var result = await _productsService.AnyAsync(id);

        //assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("1234")]
    [InlineData("61a6058e6c43f32854e51f01234")]
    public async Task AnyAsync_Should_Throw(string id)
    {
        //act & assert
        await Assert.ThrowsAnyAsync<FormatException>(() => _productsService.AnyAsync(id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("61a6058e6c43f32854e51f00")]
    [InlineData("61a6058e6c43f32854e51f01")]
    public async Task RemoveAsync_Should_Not_Throw(string id)
    {
        //arrange
        await _productsService.CreateTestProduct(productId: id);

        //act
        var exception = await Record.ExceptionAsync(() => _productsService.RemoveAsync(id));

        //assert
        Assert.Null(exception);
        Assert.False(await _productsService.AnyAsync(id));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("1234")]
    [InlineData("61a6058e6c43f32854e51f01234")]
    public async Task RemoveAsync_Should_Throw(string id)
    {
        //act
        var exception = await Record.ExceptionAsync(() => _productsService.RemoveAsync(id));

        //assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public async Task CreateAsync_Should_Not_Throw()
    {
        //arrange
        var entity = new Product()
        {
            Id = "61a6058e6c43f32854e51f00",
            ProductName = "test"
        };

        //act
        var exception = await Record.ExceptionAsync(() => _productsService.CreateAsync(entity));

        //assert
        Assert.Null(exception);
        Assert.True(await _productsService.AnyAsync(entity.Id));

        //arrange
        await _productsService.RemoveTestProduct(entity.Id);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("1234")]
    [InlineData("61a6058e6c43f32854e51f01234")]
    public async Task CreateAsync_Should_Throw(string id)
    {
        //arrange
        var entity = new Product()
        {
            Id = id,
            ProductName = "test"
        };

        //act
        var exception = await Record.ExceptionAsync(() => _productsService.CreateAsync(entity));

        //assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("1234")]
    [InlineData("61a6058e6c43f32854e51f01234")]
    public async Task UpdateAsync_Should_Throw(string id)
    {
        //arrange
        var entity = new Product()
        {
            Id = id,
            ProductName = "test"
        };

        //act
        var exception = await Record.ExceptionAsync(() => _productsService.UpdateAsync(entity.Id, entity));

        //assert
        Assert.NotNull(exception);
        Assert.IsType<FormatException>(exception);
    }

    [Fact]
    public async Task UpdateAsync_Should_Not_Throw()
    {
        //arrange
        const string productId = "61a6058e6c43f32854e51f00";
        await _productsService.CreateTestProduct(productId);
        var entity = await _productsService.GetAsync(productId);
        if(entity is null)
        {
            throw new Exception(message: "No item added");
        }

        entity.ProductName = "my new test";

        //act
        var exception = await Record.ExceptionAsync(() => _productsService.UpdateAsync(entity.Id!, entity));

        var entity_after_update = await _productsService.GetAsync(productId);
        if(entity_after_update is null)
        {
            throw new Exception(message: "No item added");
        }

        //assert
        Assert.Null(exception);
        Assert.True(entity_after_update.ProductName == "my new test");

        //arrange
        await _productsService.RemoveTestProduct(productId);
    }
}