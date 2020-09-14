using Newtonsoft.Json;
using QRCode.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRCode.Models
{
    public class Response
    {
        public bool status { get; set; }
        public string message { get; set; }
        public ResponseCode responseCode { get; set; }
    }

    public enum ResponseCode
    {
        Success = 200,
        Error = 2,
        InternalServerError = 500,
        MovedPermanently = 301,
        NotFound = 404,
        BadRequest = 400,
        Conflict = 409,
        Created = 201,
        NotAcceptable = 406,
        Unauthorized = 401,
        RequestTimeout = 408,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        Permissionserror = 403,
        Forbidden = 403,
        TokenRequired = 499,
        InvalidToken = 498
    }

    public class UsersResponse : Response { }

    public class QRUsersResponse : Response
    {
        public int userId { get; set; }
        public string email { get; set; }
    }
    public class EmailResponse : Response { }

    public class SignInResponse : Response
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string token { get; set; }
    }

    public class Pagination
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
    }
    public class InstitutionResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<InstitutionsModel> data { get; set; }
    }

    public class ErrorDetails
    {
        public int status { get; set; }
        public string detail { get; set; }
        public int code { get; set; }
    }
    public class ErrorResponse
    {
        public List<ErrorDetails> errors { get; set; }
    }
}
