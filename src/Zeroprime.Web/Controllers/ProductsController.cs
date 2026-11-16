using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Products;
using Zeroprime.Web.ViewModels.Products;

namespace Zeroprime.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    [HttpGet("products")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new ProductListViewModel(await _products.GetActiveProductsAsync(cancellationToken));
        return View(model);
    }

    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var product = await _products.GetProductByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        return View(new ProductDetailsViewModel(product));
    }
}
