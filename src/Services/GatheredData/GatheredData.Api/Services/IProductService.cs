namespace GatheredData.Api.Services;

using GatheredData.Api.Models;

public interface IProductsService
{
    public Task<List<Product>> GetAsync();

    public Task<Product?> GetAsync(string id);

    Task<bool> AnyAsync(string id);

    public Task CreateAsync(Product newObj);

    public Task UpdateAsync(string id, Product updatedObj);

    public Task RemoveAsync(string id);
}