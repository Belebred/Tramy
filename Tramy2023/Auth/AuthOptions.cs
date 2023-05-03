using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Tramy.Backend.Auth
{
    /// <summary>
    /// Auth options for JWT Authentication
    /// </summary>
    public class AuthOptions
    {
        /// <summary>
        /// Token's issuer
        /// </summary>
        public const string Issuer = "Team 21, Inc";
        /// <summary>
        /// Token's client
        /// </summary>
        public const string Audience = "Tramy Web Client";
        /// <summary>
        /// Secret key
        /// </summary>
        const string Key = "tramparamparam!123";  
        /// <summary>
        /// Lifetime in minutes
        /// </summary>
        public const int Lifetime = 43800; //TODO: change to 30

        /// <summary>
        /// Get symmetric security key
        /// </summary>
        /// <returns>Symmetric security key</returns>
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new(Encoding.ASCII.GetBytes(Key));
        }
    }
}
