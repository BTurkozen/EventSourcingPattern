﻿using EventSourcing.Api.Commands;
using EventSourcing.Api.EventStores;
using MediatR;

namespace EventSourcing.Api.Handlers.ProductHandlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly ProductStream _productStream;

        public DeleteProductCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _productStream.Deleted(request.Id);

            await _productStream.SaveAsync();

            return Unit.Value;
        }
    }
}
