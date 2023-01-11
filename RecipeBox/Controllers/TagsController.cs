using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace RecipeBox.Controllers
{
  public class TagsController : Controller
  {
    private readonly RecipeBoxContext _db;

    public TagsController(RecipeBoxContext db)
      {        
        _db = db;
      }
    
    public ActionResult Index()
    {
      List<Tag> tagsList = _db.Tags.ToList();
      return View(tagsList);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Tag tag)
    {
      if (!ModelState.IsValid)
      {
        return View(tag); 
      }
      else
      {
      _db.Tags.Add(tag);
      _db.SaveChanges();
      return RedirectToAction("Index");
      }
    }

    public ActionResult Details(int id)
    {
      Tag thisTag = _db.Tags.Include(tag => tag.JoinEntities).ThenInclude(join => join.Recipe).FirstOrDefault(tag => tag.TagId == id);
      return View(thisTag);
    }

    public ActionResult AddRecipe(int id)
    {
      Tag thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
      ViewBag.RecipeId = new SelectList (_db.Recipes, "RecipeId", "Name");
      return View(thisTag);
    }

    [HttpPost]
    public ActionResult AddRecipe(Tag tag, int recipeId) 
    {
      #nullable enable
      RecipeTag? joinEntity = _db.RecipeTags.FirstOrDefault(join => (join.RecipeId == recipeId && join.TagId == tag.TagId));
      #nullable disable

      if (joinEntity == null && recipeId != 0)
      {
        _db.RecipeTags.Add(new RecipeTag() { TagId = tag.TagId, RecipeId = recipeId});
        _db.SaveChanges();
      }

      return RedirectToAction("Details", new { id = tag.TagId});
    }

    [HttpPost]
    public ActionResult DeleteJoin(int joinId)
    {
      RecipeTag joinEntry = _db.RecipeTags.FirstOrDefault(entry => entry.RecipeTagId == joinId);
      _db.RecipeTags.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}