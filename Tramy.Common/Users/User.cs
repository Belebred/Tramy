using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Tramy.Common.Common;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Class for base user of Tramy
    /// </summary>

    public class User:BaseMongoItem
    {
        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's birthday
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// User's gender
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// User's registration date
        /// </summary>
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Code for link email to Tramy. Must be clean after use
        /// </summary>
        public string EmailActivationCode { get; set; }

        /// <summary>
        /// Code for link phone to Tramy. Must be clean after use
        /// </summary>
        public string PhoneActivationCode { get; set; }

        /// <summary>
        /// Text status of the user
        /// </summary>
        public string TextStatus { get; set; }

        /// <summary>
        /// User's registration status
        /// </summary>
        public RegStatus RegStatus { get; set; }

        /// <summary>
        /// User's status in Tramy
        /// </summary>
        public UserStatus UserStatus { get; set; }

        /// <summary>
        /// If true, UserStatus can be changed
        /// </summary>
        public bool UserStatusPerm { get; set; } = true;

        /// <summary>
        /// User's visibility in Tramy
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// User's interests
        /// </summary>
        public List<Guid> Interests { get; set; }

        /// <summary>
        /// User's account type
        /// </summary>
        [Required]
        public AccountType AccountType { get; set; }

        /// <summary>
        /// User's avatar in Base64 string
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// User's password in MD5 hash
        /// </summary>
        [JsonIgnore]
        public string PasswordHash { get; set; }

        /// <summary>
        /// User's country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// User's town
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// User's roles
        /// </summary>
        public IEnumerable<Guid> Roles { get; set; } = new List<Guid>();

        /// <summary>
        /// List of user's tracker
        /// </summary>
        public List<Guid> Trackers { get; set; }

        /// <summary>
        /// User's organization. If applicable
        /// </summary>
        public Guid? Organization { get; set; }
        
        /// <summary>
        /// Current user location
        /// </summary>
        public Guid? CurrentLocation { get; set; }

        /// <summary>
        /// Current user location part
        /// </summary>
        public Guid? CurrentLocationPart { get; set; }

        /// <summary>
        /// Current user location X
        /// </summary>
        public int? CurrentX { get; set; }

        /// <summary>
        /// Current user location Y
        /// </summary>
        public int? CurrentY { get; set; }


        /// <summary>
        /// Geo point
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] { 0, 0 };


        /// <summary>
        /// Show user position
        /// </summary>
        public bool ShowMyPositionToFriends { get; set; }

        /// <summary>
        /// Show user position
        /// </summary>
        public bool ShowMyPositionToAll { get; set; }

        /// <summary>
        /// List of UserEvents
        /// </summary>
        public List<Guid> UserEvents { get; set; } = new List<Guid>();
        /// <summary>
        /// List of UserEvents that sent invites
        /// </summary>
        public List<Guid> UserEventInvites { get; set; } = new List<Guid>();
        /// <summary>
        /// List of UserEvents where user sent request
        /// </summary>
        public List<Guid> UserEventRequests { get; set; } = new List<Guid>();

        /// <summary>
        /// Bunches, where user
        /// </summary>
        public List<Guid> Bunches { get; set; } = new List<Guid>();
        /// <summary>
        /// Invites to the bunch
        /// </summary>
        public List<Guid> BunchInvites { get; set; } = new List<Guid>();

        /// <summary>
        /// Invites from Hike to User
        /// </summary>
        public List<Guid> HikeInvites { get; set; } = new List<Guid>();

        /// <summary>
        /// User's hikes
        /// </summary>
        public List<Guid> Hikes { get; set; } = new List<Guid>();

        /// <summary>
        /// Requests to the bunches
        /// </summary>
        public List<Guid> BunchRequests { get; set; } = new List<Guid>();

        /// <summary>
        /// Requests from user to Hikes
        /// </summary>
        public List<Guid> HikeRequests { get; set; } = new List<Guid>();

        /// <summary>
        /// Parents of the user
        /// </summary>
        public List<Guid> Parents { get; set; } = new List<Guid>();

        /// <summary>
        /// Children of the user
        /// </summary>
        public List<Guid> Children { get; set; } = new List<Guid>();

        /// <summary>
        /// User's friends
        /// </summary>
        public List<Guid> Friends { get; set; } = new List<Guid>();

        /// <summary>
        /// Best friends of the user
        /// </summary>
        public List<Guid> BestFriends { get; set; } = new List<Guid>();

        /// <summary>
        /// User's incoming friends
        /// </summary>
        public List<Guid> IncomingFriends { get; set; } = new List<Guid>();

        /// <summary>
        /// User's upcoming friends
        /// </summary>
        public List<Guid> UpcomingFriends { get; set; } = new List<Guid>();

        /// <summary>
        /// User's last login date and time
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// User's notifications
        /// </summary>
        public List<Guid> Notifications { get; set; } = new List<Guid>();

        /// <summary>
        /// User's SignalR chat hub id
        /// </summary>

        public string CurrentChatHubId { get; set; }

        /// <summary>
        /// User's SignalR notification hub id
        /// </summary>

        public string CurrentNotiHubId { get; set; }


        /// <summary>
        /// Login Code by phone
        /// </summary>

        public string PhoneLoginCode { get; set; }
    }
}
