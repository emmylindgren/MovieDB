using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers
{
    public class ActorController : Controller
    {
        public IActionResult Actors()
        {
            List<ActorDetail> ActorList = new List<ActorDetail>();
            ActorMethods am = new ActorMethods();

            string error = "";
            ActorList = am.GetAllActors(out error);

            ViewBag.error = error;
            
            return View(ActorList);
        }

        public IActionResult DeleteActor(int actorId)
        {
            ActorMethods am = new ActorMethods();
            string error = "";

            int hej = actorId;
 
            am.DeleteActor(actorId, out error);
            ViewBag.error = error;

            return RedirectToAction("Actors");
        }

        [HttpGet]
        public IActionResult AddActor()
        {
            MovieMethods mm = new MovieMethods();

            string error = "";
            List<SelectListItem> movies = mm.GetAllMoviesTitleandID(out error);

            ViewBag.MovieSelection = movies;
            return View();
        }

        [HttpPost]
        public IActionResult AddActor(ActorDetail ad)
        {
            ActorMethods am = new ActorMethods();
            int i = 0;
            string error = "";

            List<String> movieee = ad.Movies;

            i = am.InsertActor(ad, out error);

            ViewBag.error = error;
            ViewBag.antal = i;

            return RedirectToAction("Actors");
        }

        public IActionResult ActorDetails(int actorID)
        {

            MovieMethods mm = new MovieMethods();
            ActorMethods am = new ActorMethods();
            string error = "";

            ActorDetail ad = new ActorDetail();

            ad = am.GetOneActor(actorID, out error);
            List<String> movies = mm.GetMoviesCorrespondingTo(ad.Id, out error);
            ViewBag.MovieList = movies;
          
            
            ViewBag.error = error;

            return View(ad);

        }
    }
}
