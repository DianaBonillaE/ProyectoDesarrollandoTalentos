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
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
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
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name");
            return View();
        }

        // POST: Teachers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,address,state,description,email,phone_number,photo_id")] Teacher teacher, Boolean state, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                teacher.state = state;

                if (File == null)
                {
                    ViewBag.MessagePhoto = "Debe ingresar una imagen";
                    return View();
                }
                else
                {

                    if (nameFile == "")
                    {
                        ViewBag.MessagePhotoName = "Debe ingresar un nombre";
                        ViewBag.MessagePhoto = "Debe ingresar una imagen";
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

                        db.Teacher.Add(teacher);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
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
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,address,state,description,email,phone_number,photo_id")] Teacher teacher, Boolean state, HttpPostedFileBase File, string nameFile)
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

                return RedirectToAction("Index");
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

                db.Photo.Remove(Photo);
                db.Teacher.Remove(teacher);

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
