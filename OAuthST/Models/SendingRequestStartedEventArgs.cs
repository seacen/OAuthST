using System;

namespace OAuthST.Models
{
    /// <summary>
    /// EventArgs of <see cref="ResourceController.AuthenticationStarted"/>
    /// </summary>
    public class SendingRequestStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The request sent for authentication
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="request"><seealso cref="Request"/></param>
        public SendingRequestStartedEventArgs(string request)
        {
            Request = request;
        }
    }
}
