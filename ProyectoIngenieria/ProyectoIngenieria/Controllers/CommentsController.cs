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
    public class CommentsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Comments
        public ActionResult Index(string message, int page = 1, int pageSize = 4)
        {
            if(message != null)
            {
                ViewBag.message = message;
            }
            List<Comments> commentList = db.Comments.ToList();
            PagedList<Comments> model = new PagedList<Comments>(commentList, page, pageSize);
            return View(model);
        }


        // GET: Comments/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,state")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                comments.state = false;
                db.Comments.Add(comments);
                db.SaveChanges();
                ViewBag.success = "Su comentario ha sido enviado exitosamente";
                return View();
            }

            return View();
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            ViewBag.description = comments.description;
            return View(comments);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,States")] Comments comments, Boolean state)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comments).State = EntityState.Modified;
                comments.state = state;
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "El comentario se editó exitosamente" });
            }
            return View(comments);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return HttpNotFound();
            }
            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comments comments = db.Comments.Find(id);
            db.Comments.Remove(comments);
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El comentario se eliminó exitosamente" });
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
