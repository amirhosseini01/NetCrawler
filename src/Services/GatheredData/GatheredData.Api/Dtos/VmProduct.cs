using System.ComponentModel.DataAnnotations;

namespace GatheredData.Api.Dtos;
public record ProductInputDto(string? Id, string? ProductName);
public class ProductPayLoadDto
{
    [Required]
    [StringLength(maximumLength: 24)]
    public string? Id { get; set; }
    [Required]
    public string ProductName { get; set; } = null!;
}