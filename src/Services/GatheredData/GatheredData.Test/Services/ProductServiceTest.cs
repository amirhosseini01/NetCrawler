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
    [InlineData("61a6058e6c43f32854e51f51")]
    public async Task GetAsync_Should_Return_Product(string id)
    {
        //act
        var result = await _productsService.GetAsync(id);

        //assert
        Assert.NotNull(result);
        Assert.IsType<Product>(result);
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
    public async Task GetAsync_Should_Throw(string id)
    {
        //act & assert
        await Assert.ThrowsAnyAsync<FormatException>(() => _productsService.GetAsync(id));
    }

    [Theory]
    [InlineData("61a6058e6c43f32854e51f51")]
    public async Task AnyAsync_Should_Return_True(string id)
    {
        //act
        var result = await _productsService.AnyAsync(id);

        //assert
        Assert.True(result);
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
    public async Task AnyAsync_Should_Throw(string id)
    {
        //act & assert
        await Assert.ThrowsAnyAsync<FormatException>(() =>  _productsService.AnyAsync(id));
    }
}