using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
  public class HomeController : Controller
  {
    private readonly RecipeBoxContext _db;

    public HomeController(RecipeBoxContext db)
      {        
        _db = db;
      }
    
    public ActionResult Index()
    {

      List<Recipe> allRecipe = _db.Recipes.ToList();
      return View(allRecipe);
    }


  }
}