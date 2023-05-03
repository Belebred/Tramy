using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Organizations
{
    /// <summary>
    /// Organization definition
    /// </summary>
    public class Organization:BaseMongoItem
    {
        /// <summary>
        /// Full name of organization
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Short name of organization
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Address for physical post
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Name of head of organization
        /// </summary>
        public string Head { get; set; }

        /// <summary>
        /// Name of contact person
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// Organization's email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Organization's phone
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// Organization's buildings
        /// </summary>

        public IEnumerable<Guid> Buildings { get; set; } = new List<Guid>();

        /// <summary>
        /// Type of organization
        /// </summary>
        [Required]
        public OrganizationType OrganizationType { get; set; }

        /// <summary>
        /// Organization status
        /// </summary>
        [Required]
        public OrganizationStatus Status { get; set; }

        /// <summary>
        /// Description of organization
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Organization's locations if applicable
        /// </summary>
        public IEnumerable<Guid> Locations { get; set; } = new List<Guid>();
    }
}
