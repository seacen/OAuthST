namespace OAuthST.Models
{
    /// <summary>
    /// Classes implementing this interface can store errors.
    /// </summary>
    public interface IErrorContainer
    {
        /// <summary>
        /// error code or error name
        /// </summary>
        string Error { get; set; }

        /// <summary>
        /// description of the error
        /// </summary>
        string ErrorDescription { get; set; }
    }
}
