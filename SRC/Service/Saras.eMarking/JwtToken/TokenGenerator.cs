
namespace TokenLibrary
{
    public static class TokenGenerator
    {
        public static TokenResponse? GetToken(TokenUserContext userContext, TokenCallerType tokenType, IAppOptions appOptions)
        {
            TokenResponse? tokenResponse = null;

            var caller = TokenCallerFactory.LoadIntegrationCaller(tokenType, appOptions);
            if (caller != null)
            {
                tokenResponse = caller.Generate(userContext);
            }

            return tokenResponse;
        }
    }

}