namespace ArticleService.Models.DTOs
{
    public class ArticleResponseDto
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public decimal PrixVente { get; set; }
        public int DureeGarantieMois { get; set; }
        public DateTime DateMiseEnStock { get; set; }
        public bool EstDisponible { get; set; }
        public string Categorie { get; set; } // Ancien champ (à conserver pour compatibilité)

        // Ajoutez ces propriétés pour la catégorie
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } // <-- AJOUTEZ CETTE PROPRIÉTÉ

        // Optionnel : informations complètes sur la catégorie
        public CategoryDto Category { get; set; }
    }
}