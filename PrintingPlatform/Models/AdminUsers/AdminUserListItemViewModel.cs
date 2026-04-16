namespace PrintingPlatform.Models.AdminUsers
{
    public class AdminUserListItemViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsBlocked { get; set; }
    }
}