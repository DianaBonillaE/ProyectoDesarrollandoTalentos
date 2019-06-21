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
        public ActionResult Index(string message, int page = 1, int pageSize = 4)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
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
            ViewBag.sponsors = db.Sponsor.ToList();

            ViewBag.users = db.User.ToList();

            ViewBag.voluntaries = db.Voluntary.ToList();
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descripcion,start_date,end_date")]
            Activity activity, List<string> sponsors, List<string> users, List<string> voluntaries, HttpPostedFileBase File, string nameFile)
        {

            //refill the lists
            List<Sponsor> sponsorReturn = db.Sponsor.ToList();
            List<User> userReturn = db.User.ToList();
            List<Voluntary> voluntaryReturn = db.Voluntary.ToList();

            if (ModelState.IsValid)
            {

                //if the sponsors list are fill
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

                //if the users list are empty
                if (users == null)
                {
                    //if the sponsors list are fill so fill the viewbag
                    if (sponsors != null)
                    {
                        System.Collections.IList listSponsors = sponsors;
                        for (int i = 0; i < listSponsors.Count; i++)
                        {
                            string id = "" + listSponsors[i];
                            Sponsor sponsor = db.Sponsor.Find(id);
                            activity.Sponsor.Add(sponsor);
                        }
                    }
                    //if the voluntaries list are fill so fill the viewbag
                    if (voluntaries != null)
                    {
                        System.Collections.IList listVoluntaries = voluntaries;
                        for (int i = 0; i < listVoluntaries.Count; i++)
                        {
                            string id = "" + listVoluntaries[i];
                            Voluntary voluntary = db.Voluntary.Find(id);
                            activity.Voluntary.Add(voluntary);
                        }
                    }
                    //if the nameFile is different of null so fill the viewbag
                    if (nameFile != "")
                    {
                        ViewBag.name = nameFile;
                    }


                    //refill the lists
                    ViewBag.sponsors = sponsorReturn;
                    ViewBag.users = userReturn;
                    ViewBag.voluntaries = voluntaryReturn;

                    ViewBag.message = "Debe seleccionar al menos un usuario";
                    return View(activity);
                }

                //if the user list are fill
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

                //if the voluntaries are fill 
                if (voluntaries != null)
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

                //if the photo file is empty
                if (File == null)
                {
                    //if the name file is different of null fill the viewbag
                    if (nameFile != "")
                    {
                        ViewBag.name = nameFile;
                    }
                    
                    //refill the lists
                    ViewBag.sponsors = sponsorReturn;
                    ViewBag.users = userReturn;
                    ViewBag.voluntaries = voluntaryReturn;
                    ViewBag.message = "Debe ingresar una imagen";
                    return View(activity);
                }

                //if the File is fill
                else
                {
                    //but the name file is empty
                    if (nameFile == "")
                    {
                        //refill the lists
                        ViewBag.sponsors = sponsorReturn;
                        ViewBag.users = userReturn;
                        ViewBag.voluntaries = voluntaryReturn;
                        ViewBag.message = "Debe ingresar un nombre de imagen";
                        return View(activity);
                    }

                    //if the file is fill and name too
                    else
                    {

                        //add the image in the folder, in the activity and database
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
                return RedirectToAction("Index", new { message = "La actividad se ingresó exitosamente" });
            }


            return View(activity);
        }

        // GET: Activities/Edit/
        public ActionResult Edit(string message,int? id)
        {

            if(message != null)
            {
                ViewBag.message = message;
            }
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
                if (users == null)
                {
                    return RedirectToAction("Edit/" + activity.id, new { message = "Debe seleccionar almenos un usuario" });
                }
                else
                {
                    if (users.Count > 0)
                    {
                        act.User.Clear();
                        System.Collections.IList listUsers = users;
                        for (int i = 0; i < listUsers.Count; i++)
                        {
                            string id = "" + listUsers[i];
                            User s = db.User.Find(id);
                            act.User.Add(s);
                        }
                    }
                }


                db.SaveChanges();

                return RedirectToAction("Index", new { message = "Se actualizó la actividad exitosamente" });

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

            return RedirectToAction("Index",new { message = "La actividad se eliminó exitosamente" });
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
