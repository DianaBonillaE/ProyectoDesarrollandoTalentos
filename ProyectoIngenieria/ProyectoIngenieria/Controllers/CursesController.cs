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
        public ActionResult Index(string message, int page = 1, int pageSize = 4)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            List<Course> curseList = db.Course.ToList();
            PagedList<Course> model = new PagedList<Course>(curseList, page, pageSize);
            return View(model);
        }

        // GET: Curses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course curse = db.Course.Find(id);
            if (curse == null)
            {
                return HttpNotFound();
            }
            return View(curse);
        }

        // GET: Curses/Create
        public ActionResult Create()
        {

            ViewBag.rooms = db.Room.ToList();
            ViewBag.teachers = db.Teacher.ToList();

            return View();
        }

        // POST: Curses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,description,name,start_date,end_date")] Course course, List<int> rooms, List<string> teacher)
        {

            if (ModelState.IsValid)
            {

                //validation of rooms
                if (rooms != null)
                {
                    if (rooms.Count > 0)
                    {
                        System.Collections.IList listRooms = rooms;
                        for (int i = 0; i < listRooms.Count; i++)
                        {
                            string id = "" + listRooms[i];
                            Room room = db.Room.Find(Int32.Parse(id));
                            course.Room.Add(room);
                        }
                    }
                }
                else
                {
                    //refill the lists
                    ViewBag.rooms = db.Room.ToList();
                    ViewBag.teachers = db.Teacher.ToList();
                    //add the teachers in the course if the teachers list is different of null
                    if (teacher != null)
                    {
                        System.Collections.IList listTeachers = teacher;
                        for (int i = 0; i < listTeachers.Count; i++)
                        {
                            string id = "" + listTeachers[i];
                            Teacher teacherDB = db.Teacher.Find(id);
                            course.Teacher.Add(teacherDB);
                        }

                    }
                    //message
                    ViewBag.message = "Debe seleccionar al menos un aula y un profesor";
                    return View(course);
                }
                //if the rooms list is different of null go to verify the list of teachers
                if (teacher != null)
                {
                    System.Collections.IList listTeachers = teacher;
                    for (int i = 0; i < listTeachers.Count; i++)
                    {
                        string id = "" + listTeachers[i];
                        Teacher teacherDB = db.Teacher.Find(id);
                        course.Teacher.Add(teacherDB);
                    }
                }
                //if the list of teachers is null so show the message
                else
                {
                    ViewBag.rooms = db.Room.ToList();
                    ViewBag.teachers = db.Teacher.ToList();
                    ViewBag.message = "Debe seleccionar al menos un profesor y un aula";
                    return View(course);
                }

                //if everything is correct so add the curse
                db.Course.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "El curso se agregó exitosamente" });
            }


            return View(course);
        }

        // GET: Curses/Edit/5
        public ActionResult Edit(string message, int? id)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Course.Include(a => a.Teacher).Include(a => a.Room).ToList().Find(a => a.id == id);
            if (course == null)
            {
                return HttpNotFound();
            }

            ViewBag.rooms = db.Room.ToList();
            ViewBag.teachers = db.Teacher.ToList();
            return View(course);
        }

        // POST: Curses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,description,name,start_date,end_date")] Course curse, List<int> rooms, List<string> teacher)
        {

            if (ModelState.IsValid)
            {
                db.Entry(curse).State = EntityState.Modified;
                Course course = db.Course.Include(a => a.Teacher).Include(a => a.Room).ToList().Find(a => a.id == curse.id);

                if (rooms != null)
                {
                    course.Room.Clear();
                    if (rooms.Count > 0)
                    {
                        System.Collections.IList listRooms = rooms;
                        for (int i = 0; i < listRooms.Count; i++)
                        {
                            string id = "" + listRooms[i];
                            Room room = db.Room.Find(Int32.Parse(id));
                            course.Room.Add(room);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Edit/" + curse.id, new { message = "El curso debe tener aulas asociadas" });

                }
                if (teacher != null)
                {
                    course.Teacher.Clear();
                    System.Collections.IList listTeachers = teacher;
                    for (int i = 0; i < listTeachers.Count; i++)
                    {
                        string id = "" + listTeachers[i];
                        Teacher teacherDB = db.Teacher.Find(id);
                        course.Teacher.Add(teacherDB);
                    }
                }
                else
                {
                    return RedirectToAction("Edit/" + curse.id, new { message = "El curso debe tener profesores asociados" });
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "El curso se actualizó exitosamente" });
            }

            return RedirectToAction("Index");
        }

        // GET: Curses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course curse = db.Course.Find(id);
            if (curse == null)
            {
                return HttpNotFound();
            }

            ViewBag.rooms = curse.Room.ToList();
            ViewBag.teachers = curse.Teacher.ToList();
            return View(curse);
        }

        // POST: Curses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course curse = db.Course.Find(id);

            curse.Room.Clear();
            curse.Teacher.Clear();

            db.Course.Remove(curse);
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
