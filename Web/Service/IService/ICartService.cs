using ModelLibrary.Dto;

namespace Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCart(string userId);
        Task<ResponseDto?> UpsetrCart(CartDto cartDto);
        Task<ResponseDto?> UpsetrCoupon(string userId, string couponCode);
        Task<ResponseDto?> DeleteCart(string userId);
        Task<ResponseDto?> DeleteDetail(int detailId);
    }
}
