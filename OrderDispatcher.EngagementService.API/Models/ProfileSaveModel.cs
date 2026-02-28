namespace OrderDispatcher.EngagementService.API.Models
{
    public class ProfileSaveModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int ImageMasterId { get; set; }
    }
}
