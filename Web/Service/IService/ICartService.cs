using ModelLibrary.Dto;

namespace Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartAsync(string userId);
        Task<ResponseDto?> UpsetrCartAsync(CartUpsertDto cartDto);
        Task<ResponseDto?> UpsetrCouponAsync(CartHeaderDto cartHeader);
        Task<ResponseDto?> DeleteCartAsync(string userId);
        Task<ResponseDto?> DeleteDetailAsync(int detailId);
    }
}
