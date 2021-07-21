using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRec.API.Features.AddPerson
{
    public class CreatePersonCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public int[] Features { get; set; }
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
                });
        }
    }

    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, bool>
    {
        public async Task<bool> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}