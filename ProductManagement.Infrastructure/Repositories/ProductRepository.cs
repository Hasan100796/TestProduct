using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ProductManagement.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("ConnectionString is not configured");
        }

        public async Task<PagedResponse<ProductDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT Id, Name, Description, Price,
                   StockQuantity, CreatedAt
            FROM dbo.Products
            ORDER BY Id DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(1)
            FROM dbo.Products;
            """;

            var offset = (page - 1) * pageSize;

            await using var connection =
                new SqlConnection(_connectionString);

            using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
            sql,
            new { Offset = offset, PageSize = pageSize },
            cancellationToken: cancellationToken));

            var products = (await multi.ReadAsync<ProductDto>()).AsList();
            var totalRecords = await multi.ReadSingleAsync<int>();

            return new PagedResponse<ProductDto>
            {
                Items = products,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
        public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT Id, Name, Description, Price,
                   StockQuantity, CreatedAt
            FROM dbo.Products
            WHERE Id = @Id;
            """;

            await using var connection =
                new SqlConnection(_connectionString);

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<ProductDto>(
                command);
        }

        public async Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO dbo.Products
                (Name, Description, Price, StockQuantity)
            OUTPUT INSERTED.Id
            VALUES
                (@Name, @Description, @Price, @StockQuantity);
            """;

            await using var connection =
                new SqlConnection(_connectionString);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Name = request.Name.Trim(),
                    Description = request.Description,
                    request.Price,
                    request.StockQuantity
                },
                cancellationToken: cancellationToken);

            return await connection.ExecuteScalarAsync<int>(command);
        }

        public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Products
            SET Name = @Name,
                Description = @Description,
                Price = @Price,
                StockQuantity = @StockQuantity
            WHERE Id = @Id;
            """;

            await using var connection =
                new SqlConnection(_connectionString);

            var command = new CommandDefinition(
                sql,
                new
                {
                    Id = id,
                    Name = request.Name.Trim(),
                    Description = request.Description,
                    request.Price,
                    request.StockQuantity
                },
                cancellationToken: cancellationToken);

            var rowsAffected = await connection.ExecuteAsync(command);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
        {
            const string sql = """
            DELETE FROM dbo.Products
            WHERE Id = @Id;
            """;

            await using var connection =
                new SqlConnection(_connectionString);

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: cancellationToken);

            var rowsAffected = await connection.ExecuteAsync(command);
            return rowsAffected > 0;
        }
    }
}
