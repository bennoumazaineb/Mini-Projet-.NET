using System.ComponentModel.DataAnnotations;

public class ArticleCreateDto
{
    [Required]
    public string Reference { get; set; }

    [Required]
    public string Designation { get; set; }

    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PrixAchat { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PrixVente { get; set; }

    [Required]
    [Range(0, 120)]
    public int DureeGarantieMois { get; set; }

    public DateTime DateMiseEnStock { get; set; } = DateTime.Now;

    public bool EstDisponible { get; set; } = true;

    // Ajout de CategoryId
    public int? CategoryId { get; set; }
}