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

namespace ProyectoIngenieria.Controllers
{
    public class PhotosController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Photos
        public ActionResult Index()
        {
            return View(db.Photo.ToList());
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        public ActionResult Create()
        {

            List<Album> albums = db.Album.ToList();
            ViewBag.albums = albums;
            return View();
        }

        // POST: Photos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,image")] Photo photo, List<int> albums, HttpPostedFileBase File)
        {
            List<Album> albumes = db.Album.ToList();
            if (ModelState.IsValid)
            {
                if (albums != null)
                {
                    if (albums.Count > 0)
                    {
                        System.Collections.IList listAlbums = albums;
                        for (int i = 0; i < listAlbums.Count; i++)
                        {
                            string id = "" + listAlbums[i];
                            Album album = db.Album.Find(id);
                            photo.Album.Add(album);

                        }
                    }
                }

                if (File == null)
                {
                    ViewBag.MessagePhoto = "Debe ingresar una imagen";
                    ViewBag.MessageList = "Debe seleccionar datos";
                    return View();
                }
                else
                {
                    var extension = Path.GetExtension(File.FileName);
                    var path = Path.Combine(Server.MapPath("/Static/"), photo.name + extension);


                    photo.name = photo.name;
                    photo.image = photo.name + extension;
                    File.SaveAs(path);
                }
                
                db.Photo.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.album = new SelectList(db.Album, "identification", "name", photo.Album);


            return View(photo);
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.image = Path.Combine("/Static/", photo.image);
            ViewBag.name = photo.name;
            return View(photo);
        }

        // POST: Photos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,image")] Photo photo, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {

                var image = db.Photo.Find(photo.id);
               
                if (File != null)
                {
                    Photo imag = db.Photo.Find(photo.id);
                    imag.name = nameFile;

                    
                    var locationStatic = Path.Combine(Server.MapPath("/Static/"));
                    System.IO.File.Delete(locationStatic + imag.image);

                    
                    var fileName = Path.GetExtension(File.FileName);
                    imag.image = nameFile + fileName;
                    var path = Path.Combine(Server.MapPath("/Static/"), nameFile + fileName);

                    File.SaveAs(path);

                    db.SaveChanges();
                }
                if (nameFile != image.name)
                {
                    var Photo = db.Photo.Find(photo.id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }

                db.SaveChanges();
               db.Entry(photo).State = EntityState.Modified;
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photo.Find(id);
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
