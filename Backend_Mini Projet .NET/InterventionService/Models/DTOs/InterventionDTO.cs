namespace InterventionService.Models.DTOs
{
    public class InterventionDTO
    {
        public int Id { get; set; }
        public int ReclamationId { get; set; }
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public DateTime InterventionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsUnderWarranty { get; set; }
        public decimal? PartsCost { get; set; }
        public decimal? LaborCost { get; set; }
        public decimal? TotalCost { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Priority { get; set; }
        public string? Location { get; set; }
        public int? EstimatedHours { get; set; }
        public string? ArticleReference { get; set; }
    }
}