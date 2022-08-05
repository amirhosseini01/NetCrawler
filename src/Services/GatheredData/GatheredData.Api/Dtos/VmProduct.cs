using System.ComponentModel.DataAnnotations;

namespace GatheredData.Api.Dtos;
public record ProductInputDto(string? Id,string? ProductName);
public class ProductPayLoadDto
{
    public string? Id { get; set; }
    public string ProductName { get; set; } = null!;
}