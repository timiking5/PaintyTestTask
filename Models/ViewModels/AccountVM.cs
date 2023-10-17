using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AccountVM
    {
        public ApplicationUser User { get; set; }
        public List<Publication> Publications { get; set; }
        /// <summary>
        /// If there is should be a follow or unfollow button
        /// </summary>
        public bool FollowButton { get; set; }
        public bool AccessDenied { get; set; } = true;
    }
}
