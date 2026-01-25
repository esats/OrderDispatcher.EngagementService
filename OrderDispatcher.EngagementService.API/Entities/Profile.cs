namespace OrderDispatcher.EngagementService.API.Entities
{
    public class Profile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public int ImageMasterId { get; set; }
    }
}
