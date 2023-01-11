using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;


namespace RecipeBox.Controllers
{

  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {        
        _userManager = userManager;
        _db = db;
    }
    
    [AllowAnonymous]
    public async Task<ActionResult> Index()
    {

      // List<Recipe> sortedList = TempData["sortedRecipe"] as List<Recipe>;
      // if(sortedList != null) 
      // {
      //   ViewBag.AllRecipes = sortedList;
      // }
      // else
      // {
      ViewBag.AllRecipes = _db.Recipes.ToList();
      
      List<Recipe> recipeList = new List<Recipe> {};
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId != null)
      {
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        recipeList = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      }
      return View(recipeList);
    }

    public ActionResult Create()
    {
      

      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe)
    {
      if (!ModelState.IsValid)
      {
        return View(recipe); 
      }
      else
      { 
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        recipe.User = currentUser;
        _db.Recipes.Add(recipe);
        _db.SaveChanges();

        return RedirectToAction("Index");
      }
    }


    [AllowAnonymous]
    public ActionResult Details(int id)
    {
      Recipe thisRecipe = _db.Recipes.Include(recipe => recipe.JoinEntities).ThenInclude(join => join.Tag).FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult AddTag(int id)
    {
      Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.TagId = new SelectList (_db.Tags, "TagId", "Diet");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddTag(Recipe recipe, int tagId) 
    {
      #nullable enable
      RecipeTag? joinEntity = _db.RecipeTags.FirstOrDefault(join => (join.TagId == tagId && join.RecipeId == recipe.RecipeId));
      #nullable disable

      if (joinEntity == null && tagId != 0)
      {
        _db.RecipeTags.Add(new RecipeTag() { TagId = tagId, RecipeId = recipe.RecipeId});
        _db.SaveChanges();
      }

      return RedirectToAction("Details", new { id = recipe.RecipeId});
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      RecipeTag joinEntry = _db.RecipeTags.FirstOrDefault(entry => entry.RecipeTagId == joinId);
      _db.RecipeTags.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Index(string sort) 
    {
    
      var sortedRecipe = new List<Recipe>();
      if(sort.ToLower().Contains("ascending")) 
      {
        sortedRecipe = _db.Recipes.OrderBy(recipe => recipe.Rating).ToList();
        // return RedirectToAction("Index", allRecipe );
      }
      else if (sort.ToLower().Contains("descending"))
      {
        sortedRecipe = _db.Recipes.OrderByDescending(recipe => recipe.Rating).ToList();
        // return RedirectToAction("Index", allRecipe );
      }

      // RouteValueDictionary dict = new RouteValueDictionary();
      // dict.Add("sortedRecipe", sortedRecipe);
      

      //TempData["sortedRecipe"] = sortedRecipe;

      //return RedirectToAction("Index");


      ViewBag.AllRecipes = sortedRecipe;
      
      List<Recipe> recipeList = new List<Recipe> {};
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId != null)
      {
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
        recipeList = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      }
      //return View(recipeList);

      return View(recipeList);
    }
  }
}