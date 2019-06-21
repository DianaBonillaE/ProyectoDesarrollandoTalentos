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

        // GET: Activities/Details/
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descripcion,start_date,end_date")]
            Activity activity, List<string> sponsors, List<string> users, List<string> voluntaries, HttpPostedFileBase File, string nameFile)
        {

            List<Sponsor> sponsorReturn = db.Sponsor.ToList();
            List<User> userReturn = db.User.ToList();
            List<Voluntary> voluntaryReturn = db.Voluntary.ToList();

            if (ModelState.IsValid)
            {
                if (sponsors != null)
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
                }
                if (users == null)
                {
                    ViewBag.MessageUsers = "Debe seleccionar al menos un usuario";
                    ViewBag.sponsors = sponsorReturn;
                    ViewBag.users = userReturn;
                    ViewBag.voluntaries = voluntaryReturn;
                    return View();
                }
                else
                {
                    System.Collections.IList listUsers = users;
                    for (int i = 0; i < listUsers.Count; i++)
                    {
                        string id = "" + listUsers[i];
                        User user = db.User.Find(id);
                        activity.User.Add(user);
                    }
                }
                if (sponsors != null)
                {
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
                }

                if (File == null)
                {
                    ViewBag.MessagePhoto = "Debe ingresar una imagen";
                    ViewBag.MessageList = "Debe seleccionar datos";
                    ViewBag.sponsors = sponsorReturn;
                    ViewBag.users = userReturn;
                    ViewBag.voluntaries = voluntaryReturn;
                    return View();
                }
                else
                {

                    if (nameFile == "")
                    {
                        ViewBag.MessagePhotoName = "Debe ingresar un nombre de imagen";
                        ViewBag.MessageList = "Debe seleccionar datos";
                        ViewBag.sponsors = sponsorReturn;
                        ViewBag.users = userReturn;
                        ViewBag.voluntaries = voluntaryReturn;
                        return View();
                    }
                    else
                    {
                        var extension = Path.GetExtension(File.FileName);
                        var path = Path.Combine(Server.MapPath("/Static/"), nameFile + extension);

                        var Photo = new DB.Photo();
                        Photo.name = nameFile;
                        Photo.image = nameFile + extension;
                        File.SaveAs(path);

                        db.Photo.Add(Photo);

                        db.Activity.Add(activity);
                        db.SaveChanges();
                       
                    }
                }
                return RedirectToAction("/Index");
            }


            return View(activity);
        }

        // GET: Activities/Edit/
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

            ViewBag.image = Path.Combine("/Static/", activity.Photo.image);
            ViewBag.name = activity.Photo.name;
            ViewBag.sponsors = sponsors;
            ViewBag.voluntaries = voluntaries;
            ViewBag.users = users;

            return View(activity);
        }

        // POST: Activities/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,descripcion,start_date,end_date,photo_id")] Activity activity, List<string> sponsors, List<string> voluntaries, List<string> users, HttpPostedFileBase File, string nameFile)
        {
            List<Sponsor> sponsorReturn = db.Sponsor.ToList();
            List<User> userReturn = db.User.ToList();
            List<Voluntary> voluntaryReturn = db.Voluntary.ToList();

            if (ModelState.IsValid)
            {

                db.Entry(activity).State = EntityState.Modified;

                Activity act = db.Activity.Include(a => a.Sponsor)
                    .Include(a => a.Voluntary).Include(a => a.User).ToList().Find(ca => ca.id == activity.id);
                var imageAct = db.Photo.Find(act.photo_id);
                //se ejecuta si hubo un cambio de imagen
                if (File != null)
                {
                    Photo image = db.Photo.Find(act.photo_id);
                    image.name = nameFile;

                    //elimina la imagen anterior
                    var locationStatic = Path.Combine(Server.MapPath("/Static/"));
                    System.IO.File.Delete(locationStatic + image.image);

                    //Escribe la nueva ruta de la imagen
                    var fileName = Path.GetExtension(File.FileName);
                    image.image = nameFile + fileName;
                    var path = Path.Combine(Server.MapPath("/Static/"), nameFile + fileName);

                    File.SaveAs(path);

                    db.SaveChanges();
                }
                if (nameFile != imageAct.name)
                {
                    var Photo = db.Photo.Find(act.photo_id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }

                act.Sponsor.Clear();
                act.Voluntary.Clear();
                act.User.Clear();

                if (sponsors != null)
                {
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
                }
                if (voluntaries != null)
                {
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


                db.SaveChanges();

                return RedirectToAction("/Index");

            }

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
            Photo Photo = db.Photo.Find(act.photo_id);

            //eliminar imagen
            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + Photo.image);

            act.Sponsor.Clear();
            act.Voluntary.Clear();
            act.User.Clear();

            db.Photo.Remove(Photo);
            db.Activity.Remove(act);
            db.SaveChanges();

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
