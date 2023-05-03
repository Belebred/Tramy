namespace Tramy.Common.CrossModels
{
    /// <summary>
    /// Class to refresh request
    /// </summary>
    public class RefreshRequest
    {
        /// <summary>
        /// Access Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
