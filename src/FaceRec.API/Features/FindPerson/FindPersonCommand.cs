using FaceRec.API.DAL;
using FaceRec.API.Features.AddPerson;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRec.API.Features.FindPerson
{
    public class FindPersonCommand : IRequest<IList<string>>
    {
        public double[] Features { get; set; }
    }

    public class FindPersonValidator : AbstractValidator<CreatePersonCommand>
    {
        public FindPersonValidator()
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

    public class FindPersonHandler : IRequestHandler<FindPersonCommand, IList<string>>
    {
        private readonly IPersonRepository _repository;
        private readonly ILogger<FindPersonHandler> _logger;

        public FindPersonHandler(IPersonRepository repository, ILogger<FindPersonHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IList<string>> Handle(FindPersonCommand request, CancellationToken cancellationToken)
        {
            var mostSimilar = new SimilarPersonsCollection();

            var entities = await _repository.GetAllAsync();
            foreach (var entity in entities)
            {
                var similarityRating = Math.Abs(1 - VectorUtils.CalculateVectorCosine(request.Features, entity.Features));
                _logger.LogDebug("The similarity rating with id: {Id} is {Similarity}", entity.Id, similarityRating);
                mostSimilar.Add(similarityRating, entity.Name);
                
            }

            return mostSimilar.GetValues();
        }
    }
}