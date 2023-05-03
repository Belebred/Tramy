using System;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Locations after search
    /// </summary>
    public class LocationUser
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
        /// X
        /// </summary>
        public int? X { get; set; }


        /// <summary>
        /// Y
        /// </summary>
        public int? Y { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public LocationUser(User user)
        {
            LastName = user.LastName ?? "user";
            Email = user.Email ?? user.Phone;
            FirstName = user.FirstName ?? "Unknown";
            Id = user.Id;
            X = user.CurrentX??0;
            Y = user.CurrentY??0;
        }
    }
}
