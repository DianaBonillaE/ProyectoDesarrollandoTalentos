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
    public class VoluntariesController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Voluntaries
        public ActionResult Index(string mensaje, int page = 1, int pageSize = 4)
        {
            List<Voluntary> volutaryList = db.Voluntary.ToList();
            PagedList<Voluntary> model = new PagedList<Voluntary>(volutaryList, page, pageSize);

            //Mensaje de exito para eliminar o editar
            if (mensaje != null)
            {
                ViewBag.menssage = mensaje;
            }

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
            ViewBag.image = Path.Combine("/Static/", voluntary.Photo.image);
            ViewBag.name = voluntary.name + " " + voluntary.last_name;

            return View(voluntary);
        }

        // GET: Voluntaries/Create
        public ActionResult Create(string mensaje)
        {
            ViewBag.photo_id = new SelectList(db.Photo, "id", "name");

            //Mensaje de id repetido
            if (mensaje != null)
            {
                ViewBag.menssage = mensaje;
            }

            return View();
        }

        // POST: Voluntaries/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,state,photo_id,email,address,phone_number,link,description")] Voluntary voluntary, Boolean state, HttpPostedFileBase File, string nameFile, string description)
        {
            var query = (from r in db.Voluntary where r.identification == voluntary.identification select r).Count();
            if (query == 1)
            {
                return RedirectToAction("Create/"+voluntary.identification, new { mensaje = "El voluntario con la identificación " + voluntary.identification + " ya se encuentra registrado en el sistema" });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    voluntary.state = state;

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

                            voluntary.description = description;
                            db.Voluntary.Add(voluntary);
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index", new { mensaje = "El voluntario " + voluntary.name + " " + voluntary.last_name + " ha sido ingresado exitosamente" });
                }
            }

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
            ViewBag.image = Path.Combine("/Static/", voluntary.Photo.image);
            ViewBag.name = voluntary.Photo.name;

            ViewBag.description = voluntary.description;
            return View(voluntary);
        }

        // POST: Voluntaries/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,description,state,photo_id,email,address,phone_number,link")] Voluntary voluntary, Boolean state, HttpPostedFileBase File, string nameFile, string description)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voluntary).State = EntityState.Modified;
                voluntary.state = state;

                if (File != null)
                {
                    if (nameFile != "")
                    {
                        var Photo = db.Photo.Find(voluntary.photo_id);
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
                    var Photo = db.Photo.Find(voluntary.photo_id);
                    Photo.name = nameFile;

                    db.SaveChanges();
                }

                voluntary.description = description;
                db.SaveChanges();

                return RedirectToAction("Index", new { mensaje = "El voluntario " + voluntary.name + " " + voluntary.last_name + " ha sido actualizado exitosamente" });
            }
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

            ViewBag.image = Path.Combine("/Static/", voluntary.Photo.image);
            ViewBag.name = voluntary.name + " " + voluntary.last_name;

            return View(voluntary);
        }

        // POST: Voluntaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            Voluntary voluntary = db.Voluntary.Find(id);
            Photo Photo = db.Photo.Find(voluntary.photo_id);

            //Valida si ya existe un voluntario asociado a una actividad

            if (voluntary.Activity.Count() >0)
            {
                List<Activity> activities = voluntary.Activity.ToList();

                for (int i=0; i > activities.Count(); i++)
                {
                 
                    voluntary.Activity.Remove(activities[i]);
                }
            }

            //eliminar imagen
            var locationStatic = Path.Combine(Server.MapPath("/Static/"));
            System.IO.File.Delete(locationStatic + Photo.image);

            db.Photo.Remove(Photo);
            db.Voluntary.Remove(voluntary);

            db.SaveChanges();
            return RedirectToAction("Index", new { mensaje = "El voluntario " + voluntary.name + " " + voluntary.last_name + " ha sido eliminado exitosamente" });

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
