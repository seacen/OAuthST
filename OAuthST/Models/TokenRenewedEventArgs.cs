using System;

namespace OAuthST.Models
{
    /// <summary>
    /// EventArgs of <see cref="AuthController.TokenRenewed"/>
    /// </summary>
    public class TokenRenewedEventArgs : EventArgs
    {
        /// <summary>
        /// <seealso cref="AuthController.Info"/>
        /// </summary>
        public OAuthResponse TokenInfo { get; set; }
    }
}
