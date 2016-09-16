using OAuthST.Models;

namespace OAuthST.ExtensionMethods
{
    /// <summary>
    /// <see cref="IErrorContainer"/> extension method
    /// </summary>
    public static class IErrorContainerEx
    {
        /// <summary>
        /// Combine error string and error description string
        /// </summary>
        public static string IErrorContainerToString(this IErrorContainer container)
        {
            return container.Error.ObjectToString("Error") + container.ErrorDescription.ObjectToString("ErrorDescription");
        }
    }
}
