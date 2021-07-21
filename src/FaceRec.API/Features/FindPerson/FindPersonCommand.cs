using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRec.API.Features.AddPerson
{
    public class FindPersonCommand : IRequest<List<string>>
    {
        public int[] Features { get; set; }
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
                });
        }
    }

    public class FindPersonHandler : IRequestHandler<FindPersonCommand, List<string>>
    {
        public Task<List<string>> Handle(FindPersonCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}