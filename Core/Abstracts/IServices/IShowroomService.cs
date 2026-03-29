using Core.Concretes.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Responses;

namespace Core.Abstracts.IServices
{
    public interface IShowroomService
    {
        Task<IResult<IEnumerable<ProductListItemDto>>> GetProductAsync();
        Task<IResult<ProductDetailDto>> GetProductDetailAsync(int id);
    }
}
