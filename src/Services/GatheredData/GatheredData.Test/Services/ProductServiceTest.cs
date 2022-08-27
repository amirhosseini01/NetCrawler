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
            ConnectionString = "mongodb://gathereddataapidb:27017",
            DatabaseName = "NetCrawler",
            ProductsCollectionName = "Products"
        };

        IOptions<ProductStoreDatabaseSettings> opt = Options.Create(productStoreDatabaseSettings);

        _productsService = new ProductsService(opt);
    }

    [Fact]
    public async Task GetAsync_Should_Return_ListOfProducts()
    {
        //arrange
         ProductStoreDatabaseSettings productStoreDatabaseSettings = new()
        {
            ConnectionString = "mongodb://gathereddataapidb:27017",
            DatabaseName = "NetCrawler",
            ProductsCollectionName = "Products"
        };

        IOptions<ProductStoreDatabaseSettings> opt = Options.Create(productStoreDatabaseSettings);

        var productsService = new ProductsService(opt);

        //act
        var result = await productsService.GetAsync();

        //assert
        Assert.NotNull(result);
        Assert.IsType<List<Product>>(result);
    }
}