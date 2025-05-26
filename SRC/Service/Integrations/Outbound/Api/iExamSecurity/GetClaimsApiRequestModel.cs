using System;
using System.Collections.Generic;
using System.Net;

namespace iExamSecurity.Util
{

    public class Body
    {
        public string mode { get; set; }
        public string raw { get; set; }
        public Options options { get; set; }
    }

    public class Header
    {
        public string key { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Info
    {
        public string _postman_id { get; set; }
        public string name { get; set; }
        public string schema { get; set; }
    }

    public class ClaimsApiRequest
    {
        public string name { get; set; }
        public Request request { get; set; }
        public List<object> response { get; set; }
        public string RandSymmKey { get; set; }
    }

    public class Options
    {
        public Raw raw { get; set; }
    }

    public class Raw
    {
        public string language { get; set; }
    }

    public class Request
    {
        public string method { get; set; }
        public List<Header> header { get; set; }
        public Body body { get; set; }
        public Url url { get; set; }
        public string ContentType { get; set; }
    }

    public class Root
    {
        public Info info { get; set; }
        public List<ClaimsApiRequest> item { get; set; }
    }

    public class Url
    {
        public string raw { get; set; }
        public string protocol { get; set; }
        public List<string> host { get; set; }
        public List<string> path { get; set; }
    }

    public class GetClainRequestBody
    {
        public string userLoginId { get; set; }
        public string currentWorkingOrganisation { get; set; }
        public string roleProfile { get; set; }

    }
    public class GetClaimResponse
    {
        public string Body { get; set; }
        public GetClaimResponseHeader Header { get; set; }
        public ClaimsDecryptedResponse ClaimsResponse { get; set; }

    }
    public class GetClaimResponseHeader
    {
        public string APPID { get; set; }
        public string nonce { get; set; }
        public string timestamp { get; set; }
        public string datakey { get; set; }
        public string signature { get; set; }

    }
    public class ClaimsDecryptedResponse
    {
        public string DataKeyPlain { get; set; }

        public Boolean IsSignVerified { get; set; }

        public String PlainBodyText { get; set; }

        public String ExceptionMessage { get; set; }
    }

    public class HttpPostResponse
    {
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; } 
    }

}

