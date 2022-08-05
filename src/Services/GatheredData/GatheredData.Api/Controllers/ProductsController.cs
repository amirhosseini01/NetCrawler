using GatheredData.Api.Models;
using GatheredData.Api.Services;
using GatheredData.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace GatheredData.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductsService _productsService;
    private readonly IMapper _mapper;

    public ProductsController(ProductsService productsService, IMapper mapper)
    {
        _productsService = productsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<ProductPayLoadDto>> Get()
    {
        var entities = await _productsService.GetAsync();

        return _mapper.Map<List<ProductPayLoadDto>>(entities);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ProductPayLoadDto>> Get(string id)
    {
        var entity = await _productsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return _mapper.Map<ProductPayLoadDto>(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ProductInputDto newObj)
    {
        var entity = _mapper.Map<Product>(newObj);

        await _productsService.CreateAsync(entity);

        ProductPayLoadDto payLoadDto = _mapper.Map<ProductPayLoadDto>(entity);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, payLoadDto);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, ProductInputDto updatedObj)
    {
        var entity = await _productsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity = _mapper.Map<Product>(updatedObj);

        entity.Id = id;

        await _productsService.UpdateAsync(id, entity);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var entity = await _productsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        await _productsService.RemoveAsync(id);

        return NoContent();
    }
}