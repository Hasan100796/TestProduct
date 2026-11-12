using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Exceptions;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.Application.Services
{
    public sealed class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public Task<PagedResponse<ProductDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1)
                throw new BadRequestException("Page number must be greater than zero.");

            if (pageSize < 1 || pageSize > 100)
                throw new BadRequestException("Page size must be between 1 and 100.");

            return _repository.GetAllAsync(page, pageSize, cancellationToken);
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {

            if (id <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    "Product ID must be greater than zero.");

            var product = await _repository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");

            return product;
        }

        public async Task<int> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadRequestException("Product name is required");

            if (request.Name.Trim().Length < 2)
                throw new BadRequestException(
                    "Product name must contain at least 2 characters.");

            if (request.Name.Trim().Length > 150)
                throw new BadRequestException(
                    "Product name cannot exceed 150 characters.");

            // Validate Description
            if (request.Description?.Length > 1000)
                throw new BadRequestException(
                    "Description cannot exceed 1000 characters.");

            if (request.Price <= 0)
                throw new BadRequestException("Price must be greater than zero.");
            if (request.StockQuantity < 0)
                throw new BadRequestException("Stock quantity cannot be negative.");
            
            return await _repository.CreateAsync(request, cancellationToken);
        }

        public async Task UpdateAsync(
        int id, UpdateProductRequest request, CancellationToken ct)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid product ID.");

            ArgumentNullException.ThrowIfNull(request);


            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadRequestException("Product name is required.");

            if (request.Name.Trim().Length < 2)
                throw new BadRequestException(
                    "Product name must contain at least 2 characters.");

            if (request.Name.Trim().Length > 150)
                throw new BadRequestException(
                    "Product name cannot exceed 150 characters.");

            // Validate Description
            if (request.Description?.Length > 1000)
                throw new BadRequestException(
                    "Description cannot exceed 1000 characters.");

            if (request.Price <= 0)
                throw new BadRequestException("Price must be greater than zero.");

            if (request.StockQuantity < 0)
                throw new BadRequestException("Stock quantity cannot be negative.");
            var updated = await _repository.UpdateAsync(id, request, ct);
           
            if (!updated)
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
           
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid product ID.");

            var deleted = await _repository.DeleteAsync(id, ct);

            if (!deleted)
                throw new NotFoundException(
                    $"Product with ID {id} was not found.");
        }
    }
}
