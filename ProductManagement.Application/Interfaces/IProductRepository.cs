
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResponse<ProductDto>> GetAllAsync(
            int page, int pageSize, CancellationToken cancellationToken);

        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id,UpdateProductRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }

  
}
