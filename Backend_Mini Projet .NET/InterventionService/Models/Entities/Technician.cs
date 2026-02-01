namespace InterventionService.Models.Entities
{
    public class Technician
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Référence à l'utilisateur dans le microservice Auth
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}