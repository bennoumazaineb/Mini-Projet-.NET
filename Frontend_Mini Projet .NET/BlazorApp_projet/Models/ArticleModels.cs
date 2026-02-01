using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PrixAchat { get; set; }
        public decimal PrixVente { get; set; }
        public int DureeGarantieMois { get; set; }
        public DateTime DateMiseEnStock { get; set; } = DateTime.Now;
        public bool EstDisponible { get; set; } = true;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

    public class ArticleCreateDto
    {
        [Required(ErrorMessage = "La référence est requise")]
        [StringLength(50, ErrorMessage = "La référence ne peut pas dépasser 50 caractères")]
        public string Reference { get; set; } = string.Empty;

        [Required(ErrorMessage = "La désignation est requise")]
        [StringLength(200, ErrorMessage = "La désignation ne peut pas dépasser 200 caractères")]
        public string Designation { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prix d'achat est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix d'achat doit être compris entre 0.01 et 1,000,000")]
        public decimal PrixAchat { get; set; }

        [Required(ErrorMessage = "Le prix de vente est requis")]
        [Range(0.01, 1000000, ErrorMessage = "Le prix de vente doit être compris entre 0.01 et 1,000,000")]
        public decimal PrixVente { get; set; }

        [Required(ErrorMessage = "La durée de garantie est requise")]
        [Range(0, 240, ErrorMessage = "La durée de garantie doit être comprise entre 0 et 240 mois")]
        public int DureeGarantieMois { get; set; }

        public DateTime DateMiseEnStock { get; set; } = DateTime.Now;
        public bool EstDisponible { get; set; } = true;

        [Required(ErrorMessage = "La catégorie est requise")]
        public int CategoryId { get; set; }
    }

    public class ArticleUpdateDto : ArticleCreateDto
    {
    }

    public class ArticleResponse
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PrixAchat { get; set; }
        public decimal PrixVente { get; set; }
        public int DureeGarantieMois { get; set; }
        public DateTime DateMiseEnStock { get; set; }
        public bool EstDisponible { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class WarrantyCheckResponse
    {
        public int ArticleId { get; set; }
        public bool IsUnderWarranty { get; set; }
    }

    public class ArticleStats
    {
        public int TotalArticles { get; set; }
        public int AvailableArticles { get; set; }
        public int OutOfStockArticles { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal AverageProfitMargin { get; set; }
        public int UnderWarrantyCount { get; set; }
        public int CategoriesCount { get; set; }
    }
}