﻿using System;
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
    public class AlbumsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        private static int idAlbum;

        // GET: Albums
        public ActionResult Index(string message, int page = 1, int pageSize = 5)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            List<Album> albumList = db.Album.ToList();
            PagedList<Album> model = new PagedList<Album>(albumList, page, pageSize);
            return View(model);
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Album.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create( string message)
        {
           
            if (message != null)
            {
                ViewBag.message = message;
            }
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,creation_date")] Album album, HttpPostedFileBase File, [Bind(Include = "id,name,image")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                
                db.Album.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "El álbum se ingresó exitosamente" });
            }

            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Album.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,creation_date")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "Se actualizó el álbum exitosamente" });
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            Album album = db.Album.Include(a => a.Photo).ToList().Find(c => c.id == id);

            List<Photo> photos = album.Photo.ToList();
            ViewBag.photos = photos;


            
            if (album == null)
            {
                return HttpNotFound();
            }
            if (photos.Count > 0)
            { return RedirectToAction("/DeleteWarning/" + id); }
            else { return View(album); }
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            Album album = db.Album.Find(id);
            db.Album.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El álbum se eliminó exitosamente" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddPhoto(int? id, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Album.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
           
            if (message != null)
            {
                ViewBag.message = message;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto([Bind(Include = "id,name,image")] Photo photo, int? id, HttpPostedFileBase File, string nameFile)
        {
            Album album = db.Album.ToList().Find(c => c.id == id);
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

                    var extension = Path.GetExtension(File.FileName);
                    var path = Path.Combine(Server.MapPath("/Static/"), photo.name + extension);


                    photo.name = photo.name;
                    photo.image = photo.name + extension;
                    File.SaveAs(path);
                    photo.Album.Add(album);
                    db.Photo.Add(photo);
                    db.SaveChanges();
                    }
                }

                
                string direccion = "/AddPhoto/" + id;
                return RedirectToAction(direccion);
            }

            ViewBag.album = new SelectList(db.Album, "identification", "name", photo.Album);
            return RedirectToAction("Index", new { message = "Fotografías añadidas exitosamente" });
        }


        public ActionResult Photos(string message, int id, int page = 1, int pageSize = 25)
        {

            if (message != null)
            {
                ViewBag.message = message;
            }

            Album album = db.Album.Include(a => a.Photo).ToList().Find(c => c.id == id);

            List<Photo> phototList = album.Photo.ToList();

            



            idAlbum = id;

            PagedList<Photo> model = new PagedList<Photo>(phototList, page, pageSize);
            return View(model);
        }

        public ActionResult DeleteWarning(int id, int page = 1, int pageSize = 5)
        {
            Album album = db.Album.Include(a => a.Photo).ToList().Find(c => c.id == id);

            List<Photo> phototList = album.Photo.ToList();


            PagedList<Photo> model = new PagedList<Photo>(phototList, page, pageSize);
            return View(model);
        }

        // GET: Photos/Delete/5
        public ActionResult DeletePhoto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Photo photo = db.Photo.Find(id);
            ViewBag.image = Path.Combine("/Static/", photo.image);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("DeletePhoto")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhotoConfirmed(int id)
        {
            

            Photo photo = db.Photo.Find(id);
            Album album;


            album = photo.Album.ToList().Find(c => c.id == idAlbum);

            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + photo.image);

            album.Photo.Remove(photo);
            db.Photo.Remove(photo);
            
            db.SaveChanges();
            return RedirectToAction("/Photos/" + idAlbum, new { message = "La foto se eliminó exitosamente" });
           
        }

        [HttpPost, ActionName("DeleteWarning")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWarning(int id)
        {


            Album album = db.Album.Find(id);

            List<Photo> photoList = album.Photo.ToList();

            for (int i = 0; i < photoList.Count; i++)
            {
                int idImage = photoList[i].id;
                Photo photo = db.Photo.Find(idImage);
                var locationStatic = Path.Combine(Server.MapPath("/Static/"));
                System.IO.File.Delete(locationStatic + photo.image);
                db.Photo.Remove(photo);
                db.SaveChanges();
            }


            db.Album.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El álbum se elimino exitosamente" });
        }

    }
}
