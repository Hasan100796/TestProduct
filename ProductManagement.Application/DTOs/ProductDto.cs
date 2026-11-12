using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.DTOs
{
    public sealed record ProductDto
    (
        int Id,
        string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt
    );
}
