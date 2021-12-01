using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieDB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers
{
    public class ActorController : Controller
    {
        private readonly IWebHostEnvironment hostingEnviroment;
        public ActorController(IWebHostEnvironment environment)
        {
            hostingEnviroment = environment;
        }
        public IActionResult Actors()
        {
            List<ActorDetail> ActorList = new List<ActorDetail>();
            ActorMethods am = new ActorMethods();

            string error = "";
            ActorList = am.GetAllActors(out error);

            ViewBag.error = error;
            
            return View(ActorList);
        }

        public IActionResult ActorSearch(IFormCollection col)
        {
            List<ActorDetail> ActorList = new List<ActorDetail>();
            ActorMethods am = new ActorMethods();

            string searchfrase = col["searchfrase"];
            string error = "";
            ActorList = am.SearchActors(out error, searchfrase);

            return View("Actors", ActorList);
        }

        public IActionResult DeleteActor(int actorId)
        {
            ActorMethods am = new ActorMethods();
            string error = "";


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

            if (ad.ProfilePicture != null)
            {
                var profilePicFileName = GetUniqueFileName(ad.ProfilePicture.FileName);
                var uploadsPath = Path.Combine(hostingEnviroment.WebRootPath, "uploads");
                var profilePicFilePath = Path.Combine(uploadsPath, profilePicFileName);
                ad.ProfilePicture.CopyTo(new FileStream(profilePicFilePath, FileMode.Create));
                ad.ProfilePicturePath = profilePicFileName;
            }
           
            i = am.InsertActor(ad, out error);

            ViewBag.error = error;
            ViewBag.antal = i;

            return RedirectToAction("Actors");
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
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
