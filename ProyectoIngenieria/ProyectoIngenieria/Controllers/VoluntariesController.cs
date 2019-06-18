using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoIngenieria.DB;
using PagedList;

namespace ProyectoIngenieria.Controllers
{
    public class VoluntariesController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Voluntaries
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            //var voluntary = db.Voluntary.Include(v => v.Photo);
            //return View(voluntary.ToList());

            List<Voluntary> voluntaryList = db.Voluntary.ToList();
            PagedList<Voluntary> model = new PagedList<Voluntary>(voluntaryList, page, pageSize);
            return View(model);
        }

        // GET: Voluntaries/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voluntary voluntary = db.Voluntary.Find(id);
            if (voluntary == null)
            {
                return HttpNotFound();
            }
            return View(voluntary);
        }

        // GET: Voluntaries/Create
        public ActionResult Create()
        {
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name");
            return View();
        }

        // POST: Voluntaries/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,state,last_name,descripcion,photo_id")] Voluntary voluntary)
        {
            if (ModelState.IsValid)
            {
                db.Voluntary.Add(voluntary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", voluntary.photo_id);
            return View(voluntary);
        }

        // GET: Voluntaries/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voluntary voluntary = db.Voluntary.Find(id);
            if (voluntary == null)
            {
                return HttpNotFound();
            }
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", voluntary.photo_id);
            return View(voluntary);
        }

        // POST: Voluntaries/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,state,last_name,descripcion,photo_id")] Voluntary voluntary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voluntary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name", voluntary.photo_id);
            return View(voluntary);
        }

        // GET: Voluntaries/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voluntary voluntary = db.Voluntary.Find(id);
            if (voluntary == null)
            {
                return HttpNotFound();
            }
            return View(voluntary);
        }

        // POST: Voluntaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Voluntary voluntary = db.Voluntary.Find(id);
            db.Voluntary.Remove(voluntary);
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
