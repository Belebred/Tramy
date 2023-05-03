using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Users;

namespace Tramy.Common.CrossModels
{
    /// <summary>
    /// 
    /// </summary>
    public class FindUserResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFriend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName => FirstName + " " + LastName;

        /// <summary>
        /// 
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanInvite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool NeedApprove { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FindUserResult()
        {

        }

        /// <summary>
        /// Method to find user's result
        /// </summary>
        /// <param name="user">User's entity</param>
        /// <param name="myUser">Your user's entity</param>
        public FindUserResult(User user, User myUser)
        {
            LastName = user.LastName??"user";
            Country = user.Country;
            Email = user.Email??user.Phone;
            FirstName = user.FirstName??"Unknown";
            Id = user.Id;
            IsFriend = user.Friends.Contains(myUser.Id);
            Town = user.Town;
            Avatar = user.Avatar;
            CanInvite = !myUser.Friends.Contains(user.Id) && !myUser.UpcomingFriends.Contains(user.Id) &&
                        !user.UpcomingFriends.Contains(myUser.Id);
            NeedApprove = myUser.IncomingFriends.Contains(user.Id);
            ShowText = myUser.Friends.Contains(user.Id) || myUser.UpcomingFriends.Contains(user.Id);
            Result = myUser.Friends.Contains(user.Id) ? "in friends" : "invitation send";
        }
    }
}
