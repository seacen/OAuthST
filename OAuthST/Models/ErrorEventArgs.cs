using System;

namespace OAuthST.Models
{
    /// <summary>
    /// Argument of events that related to error handling
    /// </summary>
    public class ErrorEventArgs : EventArgs, IErrorContainer
    {
        /// <summary>
        /// <seealso cref="IErrorContainer.Error"/>
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// <seealso cref="IErrorContainer.ErrorDescription"/>
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="error"><seealso cref="Error"/></param>
        /// <param name="errorDescription"><seealso cref="ErrorDescription"/></param>
        public ErrorEventArgs(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }
    }
}
