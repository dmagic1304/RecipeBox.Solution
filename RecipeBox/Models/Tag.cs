using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace RecipeBox.Models
{
  public class Tag
  {
    public int TagId { get; set; }
    [Required]
    public string Diet { get; set; }
    public List<RecipeTag> JoinEntities { get; } 
  }
}