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
}