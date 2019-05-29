using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProyectoIngenieria.DB;

namespace ProyectoIngenieria.Controllers
{
    public class CursesController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Curses
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            List<Curse> curseList = db.Curse.ToList();
            PagedList<Curse> model = new PagedList<Curse>(curseList, page, pageSize);
            return View(model);
        }

        // GET: Curses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curse curse = db.Curse.Find(id);
            if (curse == null)
            {
                return HttpNotFound();
            }
            return View(curse);
        }

        // GET: Curses/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Curses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,description,name,start_date,end_date")] Curse curse)
        {
            if (ModelState.IsValid)
            {
                db.Curse.Add(curse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(curse);
        }

        // GET: Curses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curse curse = db.Curse.Find(id);
            if (curse == null)
            {
                return HttpNotFound();
            }
          
            return View(curse);
        }

        // POST: Curses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,description,name,start_date,end_date")] Curse curse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(curse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(curse);
        }

        // GET: Curses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curse curse = db.Curse.Find(id);
            if (curse == null)
            {
                return HttpNotFound();
            }
            return View(curse);
        }

        // POST: Curses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Curse curse = db.Curse.Find(id);
            db.Curse.Remove(curse);
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
