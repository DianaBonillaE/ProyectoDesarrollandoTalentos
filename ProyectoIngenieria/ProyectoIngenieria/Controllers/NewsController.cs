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
    public class NewsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: News
        public ActionResult Index(string mensaje, int page = 1, int pageSize = 6)
        {
            List<News> newsList = db.News.ToList();
            PagedList<News> model = new PagedList<News>(newsList, page, pageSize);

            //Mensaje de exito para eliminar o editar
            if (mensaje != null)
            {
                ViewBag.menssage = mensaje;
            }

            return View(model);
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create(string mensaje)
        {
            //Mensaje de id repetido
            if (mensaje != null)
            {
                ViewBag.menssage = mensaje;
            }

            return View();
        }

        // POST: News/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,date,description")] News news, string albumName, string description)
        {
            if (ModelState.IsValid)
            {

                var Album = new DB.Album();
                Album.name = albumName;
                Album.description = news.description;
                Album.creation_date = news.date;

                db.Album.Add(Album);

                db.News.Add(news);

                db.SaveChanges();
                return RedirectToAction("Index", new { mensaje = "La noticia ha sido ingresado exitosamente" });
            }

            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            News news = db.News.Find(id);
            Album album = db.Album.Find(news.album_id);

            ViewBag.albumName = album.name;
            ViewBag.albumId = news.album_id;

            ViewBag.description = news.description;

            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);
        }

        // POST: News/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,date,description,album_id")] News news, string albumName)
        {
            if (ModelState.IsValid)
            {
                Album album = db.Album.Find(news.album_id);
                album.name = albumName;
                album.description = news.description;
                album.creation_date = news.date;

                db.Entry(album).State = EntityState.Modified;
                db.Entry(news).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", new { mensaje = "La noticia ha sido editada exitosamente" });
            }

            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }

            int quantity = news.Album.Photo.Count();

            if (quantity ==0)
            {
                ViewBag.photoQuantity = "0";
            }
            else
            {
                ViewBag.photoQuantity = quantity;
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            Album album = db.Album.Find(news.Album.id);

            //Valida si ya existen fotos en el albúm

            if (news.Album.Photo.Count() > 0)
            {
                news.Album.Photo.Clear();
            }

            db.Album.Remove(album);
            db.News.Remove(news);

            db.SaveChanges();

            return RedirectToAction("Index", new { mensaje = "La noticia ha sido eliminada exitosamente" });
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

