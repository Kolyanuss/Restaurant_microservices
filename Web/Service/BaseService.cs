using ModelLibrary.Dto;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Web.Models;
using Web.Service.IService;
using static Web.Utility.StaticDetails;

namespace Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MangoApi");

                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                // token
                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                ResponseDto apiResponseDto = new();
                if (apiResponse.Content.Headers.ContentType != null)
                {
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                }
                if (!apiResponse.IsSuccessStatusCode)
                {
                    apiResponseDto.IsSuccess = false;
                    apiResponseDto.Message = apiResponse.ReasonPhrase + " " + apiResponseDto.Message;
                }
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString(),
                };
                return dto;
            }
        }
    }
}
