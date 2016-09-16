namespace OAuthST.Models
{
    /// <summary>
    /// An application making protected resource requests on behalf of the resource owner and with its authorization.
    /// </summary>
    public interface IOAuthClient
    {
        /// <summary>
        /// The client identifier issued to the client during the registration process
        /// </summary>
        string ClientID { get; }

        /// <summary>
        /// The client secret.  The client MAY omit the parameter if the client secret is an empty string.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// The scope of the access request
        /// </summary>
        string AccessScope { get; }

        /// <summary>
        /// Token endpoint of the authorization server that the <see cref="IOAuthClient"/> talks to.
        /// </summary>
        string TokenEndPoint { get; }
    }
}
