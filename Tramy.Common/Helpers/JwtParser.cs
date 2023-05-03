using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Tramy.Common.Helpers
{
    /// <summary>
    /// Class to parse JWT Token
    /// </summary>
    public static class JwtParser
    {
        /// <summary>
        /// Method to parse clainms from JWT
        /// </summary>
        /// <param name="jwt">JWT Token</param>
        /// <returns></returns>
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        /// <summary>
        /// Method to parse Base64
        /// </summary>
        /// <param name="base64">Image in Base64</param>
        /// <returns></returns>
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
