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
    public class TeachersController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Teachers
        public ActionResult Index(string message,int page = 1, int pageSize = 4)
        {

            if(message!= null)
            {
                ViewBag.message = message;
            }
            List<Teacher> teacherList = db.Teacher.ToList();
            PagedList<Teacher> model = new PagedList<Teacher>(teacherList, page, pageSize);
            return View(model);
        }

        // GET: Teachers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            ViewBag.image = Path.Combine("/Static/", teacher.Photo.image);
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create(string message)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,address,state,description,email,phone_number,photo_id,link")] Teacher teacher, Boolean state, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                var query = (from r in db.Teacher where r.identification == teacher.identification select r).Count();
                if (query == 1)
                {
                    return RedirectToAction("Create", new { message = "El profesor con la identificación " + teacher.identification + " ya se encuentra registrado" });
                }
                else
                {
                    teacher.state = state;

                    if (File == null)
                    {
                        ViewBag.message = "Debe ingresar una imagen";
                        return View(teacher);
                    }
                    else
                    {

                        if (nameFile == "")
                        {
                            ViewBag.message = "Debe ingresar un nombre para la imagen";
                            return View(teacher);
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

                            db.Teacher.Add(teacher);
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index", new { message = "El profesor ha sido registrado exitosamente" });
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            ViewBag.image = Path.Combine("/Static/", teacher.Photo.image);
            ViewBag.name = teacher.Photo.name;
            ViewBag.description = teacher.description;
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,address,state,description,email,phone_number,photo_id,link")] Teacher teacher, Boolean state, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                teacher.state = state;

                if (File != null)
                {
                    if (nameFile != "")
                    {
                        var Photo = db.Photo.Find(teacher.photo_id);
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
                    var Photo = db.Photo.Find(teacher.photo_id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }

                db.SaveChanges();

                return RedirectToAction("Index", new { message = "El profesor ha sido actualizado exitosamente" });

            }

            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teacher.Find(id);

            if (teacher == null)
            {
                return HttpNotFound();
            }
            ViewBag.image = Path.Combine("/Static/", teacher.Photo.image);
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            Teacher teacher = db.Teacher.Find(id);
            Photo Photo = db.Photo.Find(teacher.photo_id);

            //eliminar imagen
            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + Photo.image);
            teacher.Course.Clear();
            db.Photo.Remove(Photo);
            db.Teacher.Remove(teacher);

            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El profesor ha sido eliminado exitosamente" });

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
