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
				ResponseDto? response = await _service.CreateProductAsync(dto);
				if (response != null && response.IsSuccess)
				{
					TempData["success"] = "Product created successfully!";
					return RedirectToAction(nameof(ProductIndex));
				}
				else
				{
					TempData["error"] = response?.Message;
				}
			}
			return View();
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
