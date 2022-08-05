namespace GatheredData.Api.Models;

public class ProductStoreDatabaseSettings
{
     public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ProductsCollectionName { get; set; } = null!;
}