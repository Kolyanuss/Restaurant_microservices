using ModelLibrary.Dto;

namespace Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartAsync(string userId);
        Task<ResponseDto?> UpsetrCartAsync(CartUpsertDto cartDto);
        Task<ResponseDto?> UpsetrCouponAsync(string userId, string couponCode);
        Task<ResponseDto?> DeleteCartAsync(string userId);
        Task<ResponseDto?> DeleteDetailAsync(int detailId);
    }
}
