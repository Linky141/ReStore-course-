namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly StoreContext _storeContext;
    private readonly IMapper mapper;
    private readonly ImageService imageService;

    public ProductsController(StoreContext storeContext, IMapper mapper, ImageService imageService)
    {
        this.imageService = imageService;
        this.mapper = mapper;
        _storeContext = storeContext;
    }

    [HttpGet]
    public async Task<ActionResult<PageList<API.Entities.Product>>> GetProducts([FromQuery] ProductParams productPatams)
    {
        var query = _storeContext.Products
        .Sort(productPatams.OrderBy)
        .Search(productPatams.SearchTerm)
        .Filter(productPatams.Brands, productPatams.Types)
        .AsQueryable();

        var products = await PageList<API.Entities.Product>.toPagedList(query, productPatams.PageNumber, productPatams.PageSize);
        Response.AddPaginationHeader(products.MetaData);
        return products;
    }

    [HttpGet("{id}", Name = "GetProduct")]
    public async Task<ActionResult<API.Entities.Product>> GetProduct(int id)
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<API.Entities.Product>> CreateProduct([FromForm] CreateProductDto productDto)
    {
        var product = mapper.Map<API.Entities.Product>(productDto);

        if (productDto.File != null)
        {
            var imageResult = await imageService.AddImageAsync(productDto.File);
            if (imageResult.Error != null)
                return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });
            product.PictureUrl = imageResult.SecureUrl.ToString();
            product.PublicId = imageResult.PublicId;
        }

        _storeContext.Products.Add(product);
        var result = await _storeContext.SaveChangesAsync() > 0;
        if (result)
            return CreatedAtRoute("GetProduct", new { Id = product.Id }, product);
        return BadRequest(new ProblemDetails { Title = "Problem creating new product" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult<API.Entities.Product>> UpdateProduct([FromForm]UpdateProductDto productDto)
    {
        var product = await _storeContext.Products.FindAsync(productDto.Id);
        if (product == null)
            return NotFound();
        mapper.Map(productDto, product);

        if (productDto.File != null)
        {
            var imageResult = await imageService.AddImageAsync(productDto.File);
            if (imageResult.Error != null)
                return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });
            if (!string.IsNullOrEmpty(product.PublicId))
                await imageService.DeleteImageAsync(product.PublicId);
            product.PictureUrl = imageResult.SecureUrl.ToString();
            product.PublicId = imageResult.PublicId;
        }

        var result = await _storeContext.SaveChangesAsync() > 0;
        if (result)
            return Ok(product);
        return BadRequest(new ProblemDetails { Title = "Problem updating product" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _storeContext.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        if (!string.IsNullOrEmpty(product.PublicId))
            await imageService.DeleteImageAsync(product.PublicId);

        _storeContext.Products.Remove(product);
        var result = await _storeContext.SaveChangesAsync() > 0;
        if (result)
            return Ok();
        return BadRequest(new ProblemDetails { Title = "Problem deleting product" });
    }
}