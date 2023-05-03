namespace Tramy.Common.Organizations
{
    /// <summary>
    /// Status of user in Tramy
    /// </summary>
    public enum OrganizationStatus
    {
        /// <summary>
        /// Organization is active
        /// </summary>
        Active, 
        
        /// <summary>
        /// Organisation has been registred
        /// </summary>
        Registred, 
        
        /// <summary>
        /// Organisation is inactive
        /// </summary>
        Inactive, 
        
        /// <summary>
        /// Organisation has been deleted
        /// </summary>
        Deleted, 
        
        /// <summary>
        /// Organization has been freezed
        /// </summary>
        Freeze
    }
}
