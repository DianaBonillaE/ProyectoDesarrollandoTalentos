using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoIngenieria.DB;
using PagedList;

namespace ProyectoIngenieria.Controllers
{
    public class ActivitiesController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Activities
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            List<Activity> activityList = db.Activity.ToList();
            PagedList<Activity> model = new PagedList<Activity>(activityList, page, pageSize);
            return View(model);
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Include(a => a.Sponsor).Include(a => a.Voluntary).Include(a => a.User).ToList().Find(c => c.id == id);

            List<Sponsor> sponsors = activity.Sponsor.ToList();
            List<Voluntary> voluntaries = activity.Voluntary.ToList();
            List<User> users = activity.User.ToList();

            ViewBag.sponsors = sponsors;
            ViewBag.voluntaries = voluntaries;
            ViewBag.users = users;

            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create()
        {
            List<Sponsor> sponsors = db.Sponsor.ToList();
            ViewBag.sponsors = sponsors;

            List<User> users = db.User.ToList();
            ViewBag.users = users;

            List<Voluntary> voluntaries = db.Voluntary.ToList();
            ViewBag.voluntaries = voluntaries;
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descripcion,start_date,end_date")]
            Activity activity, List<int> sponsors, List<int> users, List<int> voluntaries, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                if (sponsors.Count > 0)
                {
                    System.Collections.IList listSponsors = sponsors;
                    for (int i = 0; i < listSponsors.Count; i++)
                    {
                        string id = "" + listSponsors[i];
                        Sponsor sponsor = db.Sponsor.Find(id);
                        activity.Sponsor.Add(sponsor);
                    }
                }

                System.Collections.IList listUsers = users;
                for (int i = 0; i < listUsers.Count; i++)
                {
                    string id = "" + listUsers[i];
                    User user = db.User.Find(id);
                    activity.User.Add(user);
                }

                if (voluntaries.Count > 0)
                {

                    System.Collections.IList listVoluntaries = voluntaries;
                    for (int i = 0; i < listVoluntaries.Count; i++)
                    {
                        string id = "" + listVoluntaries[i];
                        Voluntary voluntary = db.Voluntary.Find(id);
                        activity.Voluntary.Add(voluntary);
                    }
                }

                var fileName = Path.GetFileName(File.FileName);
                var path = Path.Combine(Server.MapPath("/Static/"), fileName);

                var Photo = new DB.Photo();
                Photo.name = nameFile;
                Photo.image = "/Static/" + fileName;
                File.SaveAs(path);

                db.Photo.Add(Photo);

                db.Activity.Add(activity);
                db.SaveChanges();

                return RedirectToAction("/Index");
            }


            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Include(a => a.Sponsor).Include(a => a.Voluntary).Include(a => a.User).ToList().Find(c => c.id == id);

            if (activity == null)
            {
                return HttpNotFound();
            }
            List<Sponsor> sponsors = db.Sponsor.ToList();
            List<Voluntary> voluntaries = db.Voluntary.ToList();
            List<User> users = db.User.ToList();

            ViewBag.image = activity.Photo.image;
            ViewBag.name = activity.Photo.name;
            ViewBag.photo = activity.Photo.id;
            ViewBag.sponsors = sponsors;
            ViewBag.voluntaries = voluntaries;
            ViewBag.users = users;

            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,descripcion,start_date,end_date")] Activity activity, List<int> sponsors, List<int> voluntaries, List<int> users)
        {
            if (ModelState.IsValid)
            {

                db.Entry(activity).State = EntityState.Modified;

                Activity act = db.Activity.Include(a => a.Sponsor).Include(a => a.Voluntary).Include(a => a.User).ToList().Find(ca => ca.id == activity.id);
                act.Sponsor.Clear();
                act.Voluntary.Clear();
                act.User.Clear();

                if (sponsors.Count > 0)
                {
                    System.Collections.IList listSponsors = sponsors;
                    for (int i = 0; i < listSponsors.Count; i++)
                    {
                        string id = "" + listSponsors[i];
                        Sponsor s = db.Sponsor.Find(id);
                        act.Sponsor.Add(s);
                    }
                }
                if (voluntaries.Count > 0)
                {
                    System.Collections.IList listVoluntaries = voluntaries;
                    for (int i = 0; i < listVoluntaries.Count; i++)
                    {
                        string id = "" + listVoluntaries[i];
                        Voluntary s = db.Voluntary.Find(id);
                        act.Voluntary.Add(s);
                    }
                }
                if (users.Count > 0)
                {
                    System.Collections.IList listUsers = users;
                    for (int i = 0; i < listUsers.Count; i++)
                    {
                        string id = "" + listUsers[i];
                        User s = db.User.Find(id);
                        act.User.Add(s);
                    }
                }
                activity.photo_id = 13;

                db.SaveChanges();

                return RedirectToAction("/Index");

            }
            List<Sponsor> sponsorsList = db.Sponsor.ToList();
            List<Voluntary> voluntariesList = db.Voluntary.ToList();
            List<User> usersList = db.User.ToList();
            ViewBag.sponsors = sponsorsList;
            ViewBag.voluntaries = voluntariesList;
            ViewBag.users = usersList;

            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activity.Include(a => a.Sponsor).Include(a => a.Voluntary).Include(a => a.User).ToList().Find(c => c.id == id);

            List<Sponsor> sponsors = activity.Sponsor.ToList();
            List<Voluntary> voluntaries = activity.Voluntary.ToList();
            List<User> users = activity.User.ToList();

            ViewBag.sponsors = sponsors;
            ViewBag.voluntaries = voluntaries;
            ViewBag.users = users;

            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Activity act = db.Activity.Include(a => a.Sponsor).Include(a => a.Voluntary).Include(a => a.User).ToList().Find(ca => ca.id == id);

            act.Sponsor.Clear();
            act.Voluntary.Clear();
            act.User.Clear();

            //Photo photo = db.Photo.Find(act.photo_id);
            ///db.Photo.Remove(photo);
            db.Activity.Remove(act);
            db.SaveChanges();
            try
            {
                
            }
            catch (Exception e)
            {

            }

            return RedirectToAction("/Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
