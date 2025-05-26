namespace TokenLibrary
{
    internal static class TokenCallerFactory
    {
        public static ITokenTypeCaller? LoadIntegrationCaller(TokenCallerType callerType, IAppOptions appOptions)
        {
            ITokenTypeCaller? caller = null;

            switch (callerType)
            {
                case TokenCallerType.JWT:
                    {
                        caller = new JwtToken(callerType, (TokenJwtOptions)appOptions);
                    }
                    break;
                default:
                    break;
            }

            return caller;
        }

    }
}
