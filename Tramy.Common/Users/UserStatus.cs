namespace Tramy.Common.Users
{
    /// <summary>
    /// User' status in Tramy
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User is busy
        /// </summary>
        Busy = 0, 
        
        /// <summary>
        /// User is free
        /// </summary>
        Free = 1, 
        
        /// <summary>
        /// User s boring
        /// </summary>
        Boring = 2, 
        
        /// <summary>
        /// User is on its way
        /// </summary>
        OnWay = 4, 
        
        /// <summary>
        /// User is ready to meet
        /// </summary>
        ReadyToMeet = 8
    }
}
