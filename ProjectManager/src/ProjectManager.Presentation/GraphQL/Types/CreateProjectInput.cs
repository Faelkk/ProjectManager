using System.ComponentModel.DataAnnotations;

public class CreateProjectInput
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(10)]
    public string Description { get; set; } = null!;

    [MinLength(1, ErrorMessage = "Você deve informar ao menos uma skill.")]
    public List<string> Skills { get; set; } = new();

    [Required]
    [Url(ErrorMessage = "A URL do repositório deve ser válida.")]
    public string RepositoryUrl { get; set; } = null!;

    [Url(ErrorMessage = "A URL da imagem deve ser válida.")]
    public string? ThumbnailUrl { get; set; }
}
