using ProyectoIngenieria.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoIngenieria.Controllers
{
    public class HomeController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();
        public ActionResult Index()
        {
            List<Activity> activityList = db.Activity.ToList();
            List<Comments> commentList = db.Comments.ToList();
            List<News> newsList = db.News.ToList();
            List<User> userList = db.User.ToList();

            ViewBag.activities = activityList;
            ViewBag.activity = activityList[0];
            ViewBag.comments = commentList;
            ViewBag.description = commentList[0].description;
            ViewBag.name = commentList[0].name;

            commentList.Remove(commentList[0]);
            ViewBag.users = userList;
            ViewBag.news = newsList;
            ViewBag.news2 = newsList[0];


            return View();
        }

        public ActionResult About()
        {
          
            return View();
        }

        public ActionResult Contact()
        {
            
            return View();
        }

        public ActionResult AdminView()
        {

            return View();
        }

        public ActionResult UserMenu()
        {

            return View();
        }

        public ActionResult AdminMenu()
        {

            return View();
        }
        public ActionResult Galery()
        {
            

            return View();
        }
    }
}