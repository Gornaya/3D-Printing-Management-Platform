using System.Collections.Generic;

namespace PrintingPlatform.Models.AdminUsers
{
    public class AdminUsersIndexViewModel
    {
        public string? Search { get; set; }

        public List<AdminUserListItemViewModel> Users { get; set; } = new();
    }
}