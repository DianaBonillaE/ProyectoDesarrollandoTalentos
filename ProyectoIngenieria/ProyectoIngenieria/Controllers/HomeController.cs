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
            List<User> userList = db.User.ToList();

            ViewBag.activities = activityList;
            ViewBag.comments = commentList;
            ViewBag.users = userList;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}