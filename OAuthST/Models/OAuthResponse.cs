using Newtonsoft.Json;
using OAuthST.ExtensionMethods;

namespace OAuthST.Models
{
    /// <summary>
    /// OAuth response C# class wrapper
    /// </summary>
    public sealed class OAuthResponse : IErrorContainer
    {
        /// <summary>
        /// The access token issued by the OAuth server
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The type of the token issued
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The refresh token, which can be used to obtain new access tokens
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The lifetime in seconds of the access token
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        /// <summary>
        /// The scope of the access token
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// A single ASCII error code
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Human-readable ASCII text providing additional information, 
        /// used to assist the client developer in understanding the error that occurred.
        /// </summary>
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// constructor
        /// <seealso cref="IErrorContainer"/>
        /// </summary>
        public OAuthResponse(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public OAuthResponse()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>string output of the combination of <see cref="ToStringSuccessResponse"/> and <see cref="IErrorContainerEx.IErrorContainerToString(IErrorContainer)"/></returns>
        public override string ToString()
        {
            var output = ToStringSuccessResponse() + this.IErrorContainerToString();

            return output;
        }

        /// <summary>
        /// Success response
        /// </summary>
        /// <returns>string output</returns>
        public string ToStringSuccessResponse()
        {
            var output = AccessToken.ObjectToString("AccessToken") + ExpiresIn.ObjectToString("ExpiresIn") + RefreshToken.ObjectToString("RefreshToken") + TokenType.ObjectToString("TokenType") + Scope.ObjectToString("Scope");

            return output;
        }
    }
}
