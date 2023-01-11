using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public int RecipeId { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public string Ingredients { get; set; }

    [Required]
    public string Instructions { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Please enter a value between 1 and 5")]
    public int Rating { get; set; }
    public List<RecipeTag> JoinEntities { get; } 
    public ApplicationUser User { get; set; }
  }
}