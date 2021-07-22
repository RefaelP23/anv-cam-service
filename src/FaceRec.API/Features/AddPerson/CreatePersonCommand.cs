using FaceRec.API.DAL;
using FaceRec.API.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRec.API.Features.AddPerson
{
    public class CreatePersonCommand : IRequest<int>
    {
        public string Name { get; set; }
        public double[] Features { get; set; }
    }

    public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
    {
        public CreatePersonCommandValidator()
        {
            RuleFor(v => v.Features)
                .Custom((arr, ctx) =>
                {
                    if (arr.Length != 256)
                    {
                        ctx.AddFailure(ctx.PropertyName, "Features should be a vector of size 256");
                    }
                })
                .ForEach(c => c.InclusiveBetween(-1, 1));
        }
    }

    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
    {
        private readonly IPersonRepository _repository;
        private readonly ILogger<CreatePersonCommandHandler> _logger;

        public CreatePersonCommandHandler(IPersonRepository repository, ILogger<CreatePersonCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> Handle(CreatePersonCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new Person
                {
                    Name = command.Name,
                    Features = command.Features,
                };

                var id = await _repository.AddAsync(entity);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }
    }
}