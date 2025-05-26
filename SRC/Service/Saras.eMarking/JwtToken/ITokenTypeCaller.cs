namespace TokenLibrary
{
    internal interface ITokenTypeCaller
    {

        TokenCallerType CallerType { get; }

        TokenResponse Generate(TokenUserContext userContext);



    }
}
