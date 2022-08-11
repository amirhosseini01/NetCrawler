namespace GatheredData.Api.Services;

using GatheredData.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class ProductsService: IProductsService
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductsService(
        IOptions<ProductStoreDatabaseSettings> productStoreDatabaseSettings)
    {
        MongoClient mongoClient = new(
            productStoreDatabaseSettings.Value.ConnectionString);

        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(
            productStoreDatabaseSettings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<Product>(
            productStoreDatabaseSettings.Value.ProductsCollectionName);
    }

    public async Task<List<Product>> GetAsync() =>
        await _productsCollection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetAsync(string id) =>
        await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product newObj) =>
        await _productsCollection.InsertOneAsync(newObj);

    public async Task UpdateAsync(string id, Product updatedObj) =>
        await _productsCollection.ReplaceOneAsync(x => x.Id == id, updatedObj);

    public async Task RemoveAsync(string id) =>
        await _productsCollection.DeleteOneAsync(x => x.Id == id);
}