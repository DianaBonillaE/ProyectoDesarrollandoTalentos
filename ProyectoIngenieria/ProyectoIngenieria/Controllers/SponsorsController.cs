using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProyectoIngenieria.DB;

namespace ProyectoIngenieria.Controllers
{
    public class SponsorsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Sponsors
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            List<Sponsor> sponsorList = db.Sponsor.ToList();
            PagedList<Sponsor> model = new PagedList<Sponsor>(sponsorList, page, pageSize);
            return View(model);
        }

        // GET: Sponsors/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsor.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        // GET: Sponsors/Create
        public ActionResult Create()
        {
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name");
            return View();
        }

        // GET: Sponsors/CreateEnterprise
        public ActionResult CreateSponsorEnterprise()
        {
            ViewBag.type = "Empresa";
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name");
            return View();
        }

        // POST: Sponsors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identificacion,name,last_name,email,phone_number,description,photo_id")] Sponsor sponsor, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {

                if (File == null)
                {
                    ViewBag.Photo = "Debe ingresar una imagen";
                    return View();
                }
                else
                {
                    if (nameFile == "")
                    {
                        ViewBag.MessagePhotoName = "Debe ingresar un nombre de imagen";
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
                        db.SaveChanges();

                        sponsor.photo_id = Photo.id;

                        db.Sponsor.Add(sponsor);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSponsorEnterprise([Bind(Include = "identificacion,name,last_name,email,phone_number,description")] Sponsor sponsor, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {

                if (File == null)
                {
                    ViewBag.type = "Empresa";
                    ViewBag.Photo = "Debe ingresar una imagen";
                    return View();
                }
                else
                {
                    if (nameFile == "")
                    {
                        ViewBag.type = "Empresa";
                        ViewBag.MessagePhotoName = "Debe ingresar un nombre de imagen";
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
                        db.SaveChanges();

                        sponsor.photo_id = Photo.id;

                        db.Sponsor.Add(sponsor);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            return View();
        }
        

        // GET: Sponsors/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsor.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
           
            if(sponsor.last_name== "Empresa")
            {
                ViewBag.enterprise = "Empresa";
            }
            ViewBag.name = sponsor.Photo.name;
            ViewBag.image = Path.Combine("/Static/", sponsor.Photo.image);
            return View(sponsor);
        }

        // POST: Sponsors/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identificacion,name,last_name,email,phone_number,description,photo_id")] Sponsor sponsor, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sponsor).State = EntityState.Modified;

                if (File != null )
                {
                    if (nameFile != "")
                    {
                        var Photo = db.Photo.Find(sponsor.photo_id);
                        Photo.name = nameFile;

                        //elimina la imagen anterior
                        var locationStatic = Path.Combine(Server.MapPath("/Static/"));
                        System.IO.File.Delete(locationStatic + Photo.image);

                        var extension = Path.GetExtension(File.FileName);
                        var path = Path.Combine(Server.MapPath("/Static/"), nameFile + extension);
                        File.SaveAs(path);
                        Photo.image = nameFile + extension;

                        db.SaveChanges();
                    }
                }

                if (nameFile != "")
                {
                    var Photo = db.Photo.Find(sponsor.photo_id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }

                    db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", sponsor.photo_id);
            return View(sponsor);
        }

        // GET: Sponsors/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsor.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Sponsor sponsor = db.Sponsor.Find(id);

            Photo Photo = db.Photo.Find(sponsor.photo_id);

            //eliminar imagen
            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + Photo.image);

            db.Photo.Remove(Photo);
            db.Sponsor.Remove(sponsor);

            db.SaveChanges();
            return RedirectToAction("Index");
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
