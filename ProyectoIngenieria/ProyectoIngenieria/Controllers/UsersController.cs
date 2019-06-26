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
    public class UsersController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Users
        public ActionResult Index(string message, int page = 1, int pageSize = 5)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            List<User> userList = db.User.ToList();
            PagedList<User> model = new PagedList<User>(userList, page, pageSize);
            return View(model);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.image = Path.Combine("/Static/", user.Photo.image);
            ViewBag.name = user.Photo.name;
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create(string message)
        {
            
            if (message != null)
            {
                ViewBag.message = message;
            }
            return View();
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,email,phone_number,user_name,password,address,description,link")] User user, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                if (File == null)
                {
                    ViewBag.message = "Debe ingresar una imagen";
                   
                    return View();
                }
                else
                {

                    if (nameFile == "")
                    {
                        ViewBag.message = "Debe ingresar un nombre de imagen";
                       
                        return View();
                    }
                    else
                    {
                        if (nameFile == "")
                        {
                            ViewBag.message = "Debe ingresar un nombre de imagen";
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
                        user.Photo = Photo;
                        user.photo_id=Photo.id;
                        }
                    }
                    user.password = Protection.Encrypt(user.password);
                        
                db.User.Add(user);
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index", new { message = "El usuario se ingresó exitosamente" });
                    }
                    catch (Exception e)
                    {
                        ViewBag.message = "Ya existe un usuario con esta Identificación";
                        return View();
                    }
                    
                   
                }
            }

            return View(user);
        }

        // GET: Users/Edit/
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);

         

            user.password = Protection.Decrypt(user.password);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.image = Path.Combine("/Static/", user.Photo.image);
            ViewBag.name = user.Photo.name;
            return View(user);
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,email,phone_number,user_name,password,address,description,link,photo_id")] User user, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                user.password = Protection.Encrypt(user.password);

                db.Entry(user).State = EntityState.Modified;

               User us = db.User.ToList().Find(ca => ca.identification == user.identification);

                var imageAct = db.Photo.Find(us.photo_id);

                if (File != null)
                {
                    Photo image = db.Photo.Find(us.photo_id);
                    image.name = nameFile;

                  
                    var locationStatic = Path.Combine(Server.MapPath("/Static/"));
                    System.IO.File.Delete(locationStatic + image.image);

                   
                    var fileName = Path.GetExtension(File.FileName);
                    image.image = nameFile + fileName;
                    var path = Path.Combine(Server.MapPath("/Static/"), nameFile + fileName);

                    File.SaveAs(path);

                    db.SaveChanges();
                }
                if (nameFile != imageAct.name)
                {
                    var Photo = db.Photo.Find(us.photo_id);
                    Photo.name = nameFile;
                  
                   db.SaveChanges();
                    user.Photo = Photo;
                    user.photo_id = Photo.id;
                }

              


                db.SaveChanges();
                return RedirectToAction("Index", new { message = "Se actualizó el usuario exitosamente" });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            User user = db.User.Find(id);
            user.Activity.Clear();
            Photo photo = db.Photo.Find(user.photo_id);

            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + photo.image);

            db.User.Remove(user);
            db.Photo.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El usuario se eliminó exitosamente" });
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
