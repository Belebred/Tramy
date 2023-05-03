using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;
using Tramy.Common.Users;
using System.ComponentModel.DataAnnotations;

namespace Tramy.Common.UserEvent
{
    /// <summary>
    /// UserEvent definition
    /// </summary>
    public class UserEvent:BaseMongoItem
    {
        /// <summary>
        /// creator of the event
        /// </summary>
        public Guid Creator { get; set; }

        /// <summary>
        /// Date and time of the event
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// GeoPoint of the UserEvent
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] { 0, 0 };

        /// <summary>
        /// Name of the event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short description of the event
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Open or close UserEvent
        /// </summary>
        public bool Open { get; set; } = true;

        /// <summary>
        /// List of guest's request
        /// </summary>
        public List<Guid> GuestRequests { get; set; } = new List<Guid>();
        /// <summary>
        /// List of invited guests
        /// </summary>
        public List<Guid> InvitedGuest { get; set; } = new List<Guid>();
        /// <summary>
        /// List of the guests
        /// </summary>
        public List<Guid> Guests { get; set; } = new List<Guid>();
    }
}
