using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Web.Models;
using Web.Service;
using Web.Service.IService;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();
            ResponseDto? response = await _productService.GetAllProductsAsync();
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? dto = new();
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                dto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            try
            {
                var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Error: the user cannot be found");
                }
                var cartUpsertDto = new CartUpsertDto
                {
                    UserId = userId,
                    ProductId = productDto.ProductId,
                    Count = productDto.Count
                };
                var responce = await _cartService.UpsetrCartAsync(cartUpsertDto);
                if (responce == null || !responce.IsSuccess)
                {
                    throw new Exception("Server Error: Cannot update a shopping cart. " + responce?.Message);
                }
                TempData["success"] = "The product has been added to your shopping cart.";
                return RedirectToAction(nameof(CartController.CartIndex), nameof(CartController).Replace("Controller", ""));
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
            }
            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
