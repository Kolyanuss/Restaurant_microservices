using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Dto;
using Services.ShoppingCartAPI.Data;
using Services.ShoppingCartAPI.Models;

namespace Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;

        public ShoppingCartController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        public async Task<ResponseDto> Get([FromBody] string userId)
        {
            try
            {
                CartHeader? cartHeader = await _db.cartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                if (cartHeader is null)
                {
                    cartHeader = new CartHeader() { UserId = userId };
                    await _db.cartHeaders.AddAsync(cartHeader);
                    await _db.SaveChangesAsync();
                }

                List<CartDetails>? cartDetails = await _db.CartDetails
                    .Where(u => u.CartHeaderId == cartHeader.CartHeaderId).ToListAsync();
                var headerDto = _mapper.Map<CartHeaderDto>(cartHeader);
                var detailsDto = _mapper.Map<IEnumerable<CartDetailsDto>>(cartDetails);
                _response.Result = new CartDto
                {
                    CartHeader = headerDto,
                    CartDetails = detailsDto
                };
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }

        [HttpPost]
        public async Task<ResponseDto> CartUpsert(CartUpsertDto cartUpsertDto)
        {
            try
            {
                var CartHeader = await _db.cartHeaders.FirstOrDefaultAsync(u => u.UserId == cartUpsertDto.UserId);
                if (CartHeader is null)
                {
                    // add new cart
                    var newCartHeader = new CartHeader() { UserId = cartUpsertDto.UserId };
                    await _db.cartHeaders.AddAsync(newCartHeader);
                    await _db.SaveChangesAsync();

                    // add new details
                    var newCartDetail = new CartDetails()
                    {
                        CartHeaderId = newCartHeader.CartHeaderId,
                        CartHeader = newCartHeader,
                        ProductId = cartUpsertDto.ProductId,
                        Count = cartUpsertDto.Count
                    };                    
                    await _db.CartDetails.AddAsync(newCartDetail);                    
                    await _db.SaveChangesAsync();
                    return _response;
                }

                var cartDetail = await _db.CartDetails
                    .Where(u => u.CartHeaderId == CartHeader.CartHeaderId && u.ProductId == cartUpsertDto.ProductId)
                    .FirstOrDefaultAsync();
                if (cartDetail==null)
                {
                    cartDetail = new CartDetails
                    {
                        CartHeader = CartHeader,
                        CartHeaderId = CartHeader.CartHeaderId,
                        ProductId = cartUpsertDto.ProductId
                    };
                }
                cartDetail.Count = cartUpsertDto.Count;
                _db.CartDetails.Update(cartDetail);
                
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }

        [HttpPut]
        public async Task<ResponseDto> UpsertCouponCode([FromBody] string userId, string couponCode)
        {
            try
            {
                var CartHeader = await _db.cartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                if (CartHeader is null) { throw new Exception("No cart found for this user."); }
                if (CartHeader.CouponCode != couponCode)
                {
                    CartHeader.CouponCode = couponCode;
                    _db.cartHeaders.Update(CartHeader);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("deleteall/")]
        public async Task<ResponseDto> DeleteAllCart([FromBody] string userId)
        {
            try
            {
                var cartHeader = await _db.cartHeaders.FirstAsync(u => u.UserId == userId);
                _db.cartHeaders.Remove(cartHeader);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }

        [HttpDelete]
        //[Route("{detailId:int}")]
        public async Task<ResponseDto> DeleteCartDetail([FromBody] int detailId)
        {
            try
            {
                var cartDetail = await _db.CartDetails.FirstAsync(u => u.CartDetailsId == detailId);
                _db.CartDetails.Remove(cartDetail);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }
    }
}
