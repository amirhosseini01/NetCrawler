using GatheredData.Api.Services;

namespace GatheredData.Test.Services;
internal static class ProductServiceHelper
{
    public static async Task CreateTestProduct(this IProductsService productsService,
        string productId)
    {
        if (await productsService.AnyAsync(productId))
        {
            return;
        }
        await productsService.CreateAsync(new()
        {
            Id = productId,
            ProductName = $"Test {productId}"
        });
    }

    public static async Task RemoveTestProduct(this IProductsService productsService,
        string productId)
    {
        await productsService.RemoveAsync(productId);
    }
}