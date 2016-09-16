namespace OAuthST.ExtensionMethods
{
    /// <summary>
    /// object extension methods
    /// </summary>
    public static class ObjectEx
    {
        /// <summary>
        /// convert object to string with an object prompt at front
        /// </summary>
        /// <param name="obj">the object to convert string from</param>
        /// <param name="prompt">object prompt</param>
        /// <returns></returns>
        public static string ObjectToString(this object obj, string prompt)
        {
            if (obj == null)
            {
                obj = "null";
            }
            return string.Format("\n{0}:\n{1}\n", prompt, obj);
        }
    }
}
