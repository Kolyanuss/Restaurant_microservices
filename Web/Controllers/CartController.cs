using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Dto;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Web.Service.IService;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;
        public CartController(ICartService cartService, ICouponService couponService, IProductService productService)
        {
            _cartService = cartService;
            _couponService = couponService;
            _productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            try
            {
                var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Error: the user cannot be found");
                }
                var response = await _cartService.GetCartAsync(userId);
                if (response==null || !response.IsSuccess)
                {
                    throw new Exception("Error: the shopping cart cannot be found");
                }
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

                if (cartDto?.CartDetails != null)
                {
                    foreach (var detail in cartDto.CartDetails)
                    {
                        var responseProduct = await _productService.GetProductByIdAsync(detail.ProductId);
                        if (responseProduct != null && responseProduct.IsSuccess)
                        {
                            detail.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseProduct.Result));
                            if (detail.Product != null)
                            {
                                cartDto.CartHeader.CartTotal += detail.Product.Price * detail.Count;
                            }
                        }
                    }
                }

                if (cartDto?.CartHeader != null)
                {
                    var responseCoupon = await _couponService.GetCouponAsync(cartDto.CartHeader.CouponCode);
                    if (responseCoupon != null && responseCoupon.IsSuccess)
                    {
                        var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseCoupon.Result));
                        if (coupon != null)
                        {
                            if (cartDto.CartHeader.CartTotal > coupon.MinAmount)
                            {
                                cartDto.CartHeader.Discount = coupon.DiscountAmount;
                                cartDto.CartHeader.CartTotal -= cartDto.CartHeader.Discount;
                            }
                        }
                    }
                }

                return View(cartDto);
            }
            catch (Exception e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
            }
        }
    }
}
