namespace Tramy.Common.Users
{
    /// <summary>
    /// Status of user's visibility in Tramy
    /// </summary>
    public enum VisibleStatus
    {
        /// <summary>
        /// Everybody can see you
        /// </summary>
        All = 0, 
        
        /// <summary>
        /// Only friends can see you
        /// </summary>
        Friends = 1, 
        
        /// <summary>
        /// Friends and friends of friends can see you
        /// </summary>
        FriendsOfFriends = 2, 
        
        /// <summary>
        /// Nobody can see you
        /// </summary>
        Nobody = 4
    }
}