using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Dto;
using Newtonsoft.Json;
using Web.Service;
using Web.Service.IService;

namespace Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _service;

		public ProductController(IProductService service)
		{
			_service = service;
		}

		public async Task<IActionResult> ProductIndex()
		{
			List<ProductDto>? list = new();
			ResponseDto? response = await _service.GetAllProductsAsync();
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(list);
		}

		public IActionResult ProductCreate()
		{
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto dto)
        {
			if (ModelState.IsValid)
			{
				try
				{
					ResponseDto? response = await _service.CreateProductAsync(dto);

					if (response == null || !response.IsSuccess)
					{
						TempData["error"] = response?.Message ?? "An error occurred while creating the product.";
					}
					TempData["success"] = "Product created successfully!";
					return RedirectToAction(nameof(ProductIndex));
				}
				catch (Exception ex)
				{
					TempData["error"] = "An internal error occurred. Please try again later.";
				}
			}
            return View(dto);
        }

        public async Task<IActionResult> ProductEdit(int id)
		{
			ResponseDto response = await _service.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            TempData["error"] = response?.Message;
			return NotFound();
        }

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDto dto)
		{
			if (ModelState.IsValid)
			{
				ResponseDto? response = await _service.UpdateProductAsync(dto);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Product updated successfully!";
					return RedirectToAction(nameof(ProductIndex));
				}
				TempData["error"] = response?.Message;
			}
			return View(dto);
		}

		public async Task<IActionResult> ProductDelete(int id)
		{
			ResponseDto? responseDelete = await _service.DeleteProductAsync(id);
			if (responseDelete == null || !responseDelete.IsSuccess)
			{
				TempData["error"] = responseDelete?.Message;
				return RedirectToAction(nameof(ProductIndex));
			}
			
			TempData["success"] = "Product deleted successfully!";
			return RedirectToAction(nameof(ProductIndex));
		}
	}
}
