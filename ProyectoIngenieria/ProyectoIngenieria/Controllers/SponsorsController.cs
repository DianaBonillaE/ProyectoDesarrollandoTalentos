using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoIngenieria.DB;

namespace ProyectoIngenieria.Controllers
{
    public class SponsorsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Sponsors
        public ActionResult Index()
        {
            var sponsor = db.Sponsor.Include(s => s.Photo);
            return View(sponsor.ToList());
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

        // POST: Sponsors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identificacion,name,last_name,email,phone_number,description,photo_id")] Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                db.Sponsor.Add(sponsor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", sponsor.photo_id);
            return View(sponsor);
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
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", sponsor.photo_id);
            return View(sponsor);
        }

        // POST: Sponsors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identificacion,name,last_name,email,phone_number,description,photo_id")] Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sponsor).State = EntityState.Modified;
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
