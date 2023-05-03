namespace Tramy.Common.CrossModels
{
    /// <summary>
    /// Login cross model
    /// </summary>
    public class LoginResultModel
    {
        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's surname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Is user admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Login error
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// User's id
        /// </summary>
        public string Id { get; set; }
    }
}
