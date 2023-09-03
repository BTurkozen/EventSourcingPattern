using EventSourcing.Api.Dtos;
using EventSourcing.Api.Models;
using EventSourcing.Api.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Api.Handlers
{
    public class GetProductAllListByUserIdQueryHandler : IRequestHandler<GetProductAllListByUserIdQuery, List<ProductDto>>
    {
        private readonly DataContext _dataContext;

        public GetProductAllListByUserIdQueryHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<List<ProductDto>> Handle(GetProductAllListByUserIdQuery request, CancellationToken cancellationToken)
        {
            var products = _dataContext.Products
                                       .Where(p =>
                                              p.UserId == request.UserId)
                                       .Select(p => new ProductDto
                                       {
                                           UserId = request.UserId,
                                           Id = p.Id,
                                           Name = p.Name,
                                           Price = p.Price,
                                           Stock = p.Stock,
                                       })
                                       .ToListAsync();

            return products;
        }
    }
}
