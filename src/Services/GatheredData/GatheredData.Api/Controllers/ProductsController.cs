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
    private const string InvalidIdErrorMessage = $"Enter id correctly. Not null or empty and has 24 character length";
    private readonly IProductsService _productsService;
    private readonly IMapper _mapper;

    public ProductsController(IProductsService productsService, IMapper mapper)
    {
        _productsService = productsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductPayLoadDto>>> Get()
    {
        var entities = await _productsService.GetAsync();

        return _mapper.Map<List<ProductPayLoadDto>>(entities);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ProductPayLoadDto>> Get(string id)
    {
        if(!IsValidMongoDBId(id))
        {
            return BadRequest(InvalidIdErrorMessage);
        }
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
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _productsService.AnyAsync(newObj.Id!))
        {
            return BadRequest($"Enter the {nameof(newObj.Id)}");
        }


        var entity = _mapper.Map<Product>(newObj);

        await _productsService.CreateAsync(entity);

        ProductPayLoadDto payLoadDto = _mapper.Map<ProductPayLoadDto>(entity);

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, payLoadDto);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, ProductInputDto updatedObj)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
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
        if(!IsValidMongoDBId(id))
        {
            return BadRequest(InvalidIdErrorMessage);
        }
        if (!await _productsService.AnyAsync(id))
        {
            return NotFound();
        }

        await _productsService.RemoveAsync(id);

        return NoContent();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public bool IsValidMongoDBId(string id)
    {
        return !string.IsNullOrEmpty(id) && id.Length == 24;
    }
}