namespace Firepuma.CommandsAndQueries.Abstractions.Authorization
{
    public abstract class AbstractRequestAuthorizer<TRequest> : IAuthorizer<TRequest>
    {
        private readonly HashSet<IAuthorizationRequirement> _requirements = new();

        public IEnumerable<IAuthorizationRequirement> Requirements => _requirements;

        protected void UseRequirement(IAuthorizationRequirement requirement)
        {
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));
            _requirements.Add(requirement);
        }

        public abstract Task BuildPolicy(TRequest request, CancellationToken cancellationToken);
    }
}