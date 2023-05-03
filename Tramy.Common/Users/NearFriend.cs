using System;

namespace Tramy.Common.Users
{
    /// <summary>
    /// User after search
    /// </summary>
    public class NearFriend
    {
        /// <summary>
        /// Id of user
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Name of user
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string FullName => FirstName ?? "Unknown" + " " + (LastName ?? "user");

        /// <summary>
        /// Geo point
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] { 0, 0 };


        /// <summary>
        /// Distance to me
        /// </summary>
        public double Distance { get; set; }
    }
}
