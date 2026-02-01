using ArticleService.Models;

public class Article
{
    public int Id { get; set; }
    public string Reference { get; set; }
    public string Designation { get; set; }
    public string Description { get; set; }
    public decimal PrixAchat { get; set; }
    public decimal PrixVente { get; set; }
    public int DureeGarantieMois { get; set; }
    public DateTime DateMiseEnStock { get; set; } = DateTime.Now;
    public bool EstDisponible { get; set; } = true;

    // Relation avec Category
    public int? CategoryId { get; set; }
    public virtual Category Category { get; set; }

    // Champs d'audit
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}