using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Services
{
    public interface IProductService
    {
        Task<PagedResponse<ProductDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);

        Task UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
