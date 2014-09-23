using System;
using System.Net;

namespace NewRelic.Platform.Sdk.Utils
{
    public class NewRelicServiceException : Exception
    {
        private HttpStatusCode _statusCode;
        private string _bodyContents;

        public HttpStatusCode StatusCode { get { return _statusCode; } }
        public string BodyContents { get { return _bodyContents; } }

        public NewRelicServiceException(HttpStatusCode statusCode, string body, Exception exception) : base(exception.Message, exception)
        {
            _statusCode = statusCode;
            _bodyContents = body;
        }
    }
}
