using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Dto;
using Services.ProductAPI.Data;
using Services.ProductAPI.Models;
using System.Threading.Tasks;

namespace Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	//[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly AppDbContext _db;
		private IMapper _mapper;
		private ResponseDto _response;

		public ProductController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
			_response = new();
		}

		[HttpGet]
		public async Task<ResponseDto> Get()
		{
			try
			{
				var objList = await _db.Products.ToListAsync();
				_response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
			}
			catch (Exception e)
			{
				_response.IsSuccess = false;
				_response.Message = e.Message;
			}
			return _response;
		}

		[HttpGet]
		[Route("{id:int}")]
		public async Task<ResponseDto> Get(int id)
		{
			try
			{
				var obj = await _db.Products.FirstAsync(u => u.ProductId == id);
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception e)
			{
				_response.IsSuccess = false;
				_response.Message = e.Message;
			}
			return _response;
		}

		[HttpPost]
		[Authorize(Roles = "ADMIN")]
		public async Task<ResponseDto> Post([FromBody] ProductDto dto)
		{
			try
			{
				Product model = _mapper.Map<Product>(dto);
				_db.Products.Add(model);
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
		[Authorize(Roles = "ADMIN")]
		public async Task<ResponseDto> Put([FromBody] ProductDto dto)
		{
			try
			{
				Product model = _mapper.Map<Product>(dto);
				_db.Products.Update(model);
				await _db.SaveChangesAsync();

				//_response.Result = _mapper.Map<ProductDto>(_db.Products.First(u => u.ProductId == dto.ProductId));
			}
			catch (Exception e)
			{
				_response.IsSuccess = false;
				_response.Message = e.Message;
			}
			return _response;
		}

		[HttpDelete]
		[Route("{id:int}")]
		[Authorize(Roles = "ADMIN")]
		public async Task<ResponseDto> Delete(int id)
		{
			try
			{
				Product obj = await _db.Products.FirstAsync(u => u.ProductId == id);
				_db.Products.Remove(obj);
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
