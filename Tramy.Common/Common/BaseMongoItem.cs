using System;
using System.ComponentModel.DataAnnotations;

namespace Tramy.Common.Common
{
    /// <summary>
    /// Class for all MongoDB items
    /// </summary>
    public abstract class BaseMongoItem
    {
        /// <summary>
        /// Id of item in collection
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();


        /// <summary>
        /// Item's creation date. By default in Now
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;

    }
}
