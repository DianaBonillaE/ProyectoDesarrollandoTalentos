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
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
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
         /*   ViewBag.image = Path.Combine("/Static/", user.Photo.image);
            ViewBag.name = user.Photo.name;*/
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
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
                    ViewBag.MessagePhoto = "Debe ingresar una imagen";
                    ViewBag.MessageList = "Debe seleccionar datos";
                    return View();
                }
                else
                {

                    if (nameFile == "")
                    {
                        ViewBag.MessagePhotoName = "Debe ingresar un nombre de imagen";
                        ViewBag.MessageList = "Debe seleccionar datos";
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
                    user.password = Protection.Encrypt(user.password);
                        
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
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

            ViewBag.image = Path.Combine("/Static/", user.Photo.image);
            ViewBag.name = user.Photo.name;

            user.password = Protection.Decrypt(user.password);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,email,phone_number,user_name,password,address,description,link,photoId")] User user, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                user.password = Protection.Encrypt(user.password);

                db.Entry(user).State = EntityState.Modified;

                var imageAct = db.Photo.Find(user.photo_id);

                if (File != null)
                {
                    Photo image = db.Photo.Find(user.photo_id);
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
                    var Photo = db.Photo.Find(user.photo_id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }




                db.SaveChanges();
                return RedirectToAction("Index");
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
            Photo photo = db.Photo.Find(user.photo_id);
            db.User.Remove(user);
            db.Photo.Remove(photo);
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
