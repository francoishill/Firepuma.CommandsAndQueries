using Firepuma.CommandsAndQueries.Abstractions.Authorization;

// ReSharper disable UnusedType.Global

namespace Sample.CommandsAndQueriesApi.MongoDb.AuthorizationRequirements;

public class PetNameMustBeAllowedRequirement : IAuthorizationRequirement
{
    private readonly string _petName;

    public PetNameMustBeAllowedRequirement(string petName)
    {
        _petName = petName;
    }

    public class AuthorizationHandler : IAuthorizationHandler<PetNameMustBeAllowedRequirement>
    {
        public async Task<AuthorizationResult> Handle(PetNameMustBeAllowedRequirement request, CancellationToken cancellationToken)
        {
            var disallowedNames = new[] { "Joe", "Soap" };
            if (disallowedNames.Contains(request._petName))
            {
                return AuthorizationResult.Fail($"Pet name is not allowed, the following names are disallowed: {string.Join(", ", disallowedNames)}");
            }

            await Task.CompletedTask;
            return AuthorizationResult.Succeed();
        }
    }
}