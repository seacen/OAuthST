using System;

namespace OAuthST.Models
{
    /// <summary>
    /// EventArgs of events when a resource controller action is succeeded
    /// </summary>
    public class ResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Resource server response
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="response"><seealso cref="Response"/></param>
        public ResponseReceivedEventArgs(string response)
        {
            Response = response;
        }
    }
}
