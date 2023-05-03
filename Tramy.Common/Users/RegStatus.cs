namespace Tramy.Common.Users
{
    /// <summary>
    /// Registration status of user in Tramy
    /// </summary>
    public enum RegStatus
    {
        /// <summary>
        /// User is active
        /// </summary>
        Active = 0, 
        
        /// <summary>
        /// User has been registred
        /// </summary>
        Registred = 1, 
        
        /// <summary>
        /// User is inactive
        /// </summary>
        Inactive = 2, 
        
        /// <summary>
        /// User has been deleted
        /// </summary>
        Deleted = 4, 
        
        /// <summary>
        /// User has been freezed
        /// </summary>
        Freeze = 8
    }
}
