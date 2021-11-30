using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieDB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace MovieDB.Controllers
{
    public class MovieController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            List<MovieDetail> MovieList = new List<MovieDetail>();
            List<SelectListItem> GenreList = new List<SelectListItem>();
            MovieMethods mm = new MovieMethods();
            GenreMethods gm = new GenreMethods();

            string error = "";
            MovieList = mm.GetAllMovies(out error);
            GenreList = gm.GetAllGenres(out error);

            ViewBag.error = error;
            ViewBag.genreList = GenreList;

            return View(MovieList);
        }

        [HttpPost]
        public IActionResult Index(string Genre)
        {
            int i = Convert.ToInt32(Genre);
            ViewBag.ChosenGenre = i;

            List<MovieDetail> MovieList = new List<MovieDetail>();
            List<SelectListItem> GenreList = new List<SelectListItem>();
            MovieMethods mm = new MovieMethods();
            GenreMethods gm = new GenreMethods();

            string error = "";
            MovieList = mm.GetAllMoviesChosenGenre(out error,i);
            GenreList = gm.GetAllGenres(out error);

            ViewBag.error = error;
            ViewBag.genreList = GenreList;

            return View(MovieList);
        }


        [HttpGet]
        public IActionResult IndexSorting(string sortBy, bool firstTime, int genre)
        {
            
            int i = Convert.ToInt32(genre);
            ViewBag.ChosenGenre = i;

            switch (sortBy)
            {
                case "Title":
                    ViewBag.FirsttimeTitle = !firstTime;
                    break;
                case "Year":
                    ViewBag.FirsttimeYear = !firstTime;
                    break;
                case "Language":
                    ViewBag.FirsttimeLang = !firstTime;
                    break;
                case "Grade":
                    ViewBag.FirsttimeGrade = !firstTime;
                    break;
                case "Genre":
                    ViewBag.FirsttimeGenre = !firstTime;
                    break;
            }

            List<MovieDetail> MovieList = new List<MovieDetail>();
            List<SelectListItem> GenreList = new List<SelectListItem>();
            MovieMethods mm = new MovieMethods();
            GenreMethods gm = new GenreMethods();

            string error = "";
            MovieList = mm.GetAllMoviesSorted(out error, sortBy, firstTime, i);
            
            GenreList = gm.GetAllGenres(out error);

            ViewBag.error = error;
            ViewBag.genreList = GenreList;

            return View("Views/Movie/Index.cshtml", MovieList);
        }


        [HttpGet]
        public IActionResult AddMovie()
        {
            LanguageMethods lm = new LanguageMethods();
            GenreMethods gm = new GenreMethods();
            ActorMethods am = new ActorMethods();
            string error = "";
            List<SelectListItem> languages = lm.GetAllLanguages(out error);
            List<SelectListItem> genres = gm.GetAllGenres(out error);
            List<SelectListItem> actors = am.GetActorandID(out error);

            ViewBag.LanguageSelection = languages;
            ViewBag.GenreSelection = genres;
            ViewBag.ActorSelection = actors;

            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(MovieDetail md)
        {
            MovieMethods mm = new MovieMethods();
            int i = 0;
            string error = "";

            i = mm.InsertMovie(md, out error);

            ViewBag.error = error;
            ViewBag.antal = i;

            return RedirectToAction("Index","Movie");
        }

        public IActionResult DeleteMovie(int id)
        {
            MovieMethods mm = new MovieMethods();
            string error = "";

            mm.DeleteMovie(id, out error);

            ViewBag.error = error;

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            MovieMethods mm = new MovieMethods();
            ActorMethods am = new ActorMethods();
            string error = "";

            MovieDetail md = new MovieDetail();

            md = mm.GetOneMovie(id, out error);
            List<String> actors = am.GetActorsCorrespondingTo(md.Id, out error);
            if(actors != null)
            {
                ViewBag.ActorList = actors;
            }
            else
            {
                ViewBag.ActorList = null;
            }
            
            ViewBag.error = error;

            return View(md);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            MovieMethods mm = new MovieMethods();
            string error = "";

            MovieDetail md = new MovieDetail();

            md = mm.GetOneMovie(id, out error);

            LanguageMethods lm = new LanguageMethods();
            GenreMethods gm = new GenreMethods();
            ActorMethods am = new ActorMethods();

            List<SelectListItem> languages = lm.GetAllLanguages(out error);
            List<SelectListItem> genres = gm.GetAllGenres(out error);
            List<SelectListItem> actors = am.GetActorandIDSelected(out error, md.Id);
            
            ViewBag.LanguageSelection = languages;
            ViewBag.GenreSelection = genres;
            ViewBag.ActorSelection = actors;
            return View(md);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] MovieDetail md)
        {
            MovieMethods mm = new MovieMethods();
            int i = 0;
            string error = "";

            i = mm.UpdateMovie(md, out error);
            ViewBag.error = error;
            ViewBag.antal = i;

            return RedirectToAction("Details", new { id = md.Id });

        }
    }
}
