using API.Data;
using API.Entities;
using API.Extensions;
using API.RequestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly StoreContext _storeContext;

        public ProductsController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet]
        public async Task<ActionResult<PageList<Product>>> GetProducts([FromQuery] ProductParams productPatams)
        {
            var query = _storeContext.Products
            .Sort(productPatams.OrderBy)
            .Search(productPatams.SearchTerm)
            .Filter(productPatams.Brands, productPatams.Types)
            .AsQueryable();

            var products = await PageList<Product>.toPagedList(query, productPatams.PageNumber, productPatams.PageSize);
            Response.AddPaginationHeader(products.MetaData);
            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _storeContext.Products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var brands = await _storeContext.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await _storeContext.Products.Select(p => p.Type).Distinct().ToListAsync();
            return Ok(new { brands, types });
        }
    }
}