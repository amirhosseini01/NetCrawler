using GatheredData.Api.Models;
using GatheredData.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatheredData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _productsService;

    public ProductsController(ProductsService productsService) =>
        _productsService = productsService;

    [HttpGet]
    public async Task<List<Product>> Get() =>
        await _productsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var obj = await _productsService.GetAsync(id);

        if (obj is null)
        {
            return NotFound();
        }

        return obj;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Product newObj)
    {
        await _productsService.CreateAsync(newObj);

        return CreatedAtAction(nameof(Get), new { id = newObj.Id }, newObj);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Product updatedObj)
    {
        var obj = await _productsService.GetAsync(id);

        if (obj is null)
        {
            return NotFound();
        }

        updatedObj.Id = obj.Id;

        await _productsService.UpdateAsync(id, updatedObj);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var obj = await _productsService.GetAsync(id);

        if (obj is null)
        {
            return NotFound();
        }

        await _productsService.RemoveAsync(id);

        return NoContent();
    }
}