using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Roles in Tramy
    /// </summary>
    public class Role:BaseMongoItem
    {
        /// <summary>
        /// Role's name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
