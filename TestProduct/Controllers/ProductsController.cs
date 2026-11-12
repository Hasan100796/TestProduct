using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;

namespace TestProduct.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto?>> GetById(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _productService.CreateAsync(request, cancellationToken);
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            return
                CreatedAtAction
                (
                nameof(GetById),
                    new { id },
                    product
            );

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
       int id,
       [FromBody] UpdateProductRequest request,
       CancellationToken ct)
        {
            await _productService.UpdateAsync(id, request, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id, CancellationToken ct)
        {
             await _productService.DeleteAsync(id, ct);

            return NoContent();
        }
    }
}
