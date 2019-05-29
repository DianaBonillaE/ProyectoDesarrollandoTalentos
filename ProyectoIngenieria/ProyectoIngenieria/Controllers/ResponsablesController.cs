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
    public class ResponsablesController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Responsables
        public ActionResult Index(int page = 1, int pageSize = 4)
        {
            List<Responsable> responsableList = db.Responsable.ToList();
            PagedList<Responsable> model = new PagedList<Responsable>(responsableList, page, pageSize);
            return View(model);
        }

        // GET: Responsables/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);

            List<Student> students = responsable.Student.ToList();
            ViewBag.students = students;
            if (responsable == null)
            {
                return HttpNotFound();
            }
            return View(responsable);
        }

        // GET: Responsables/Create
        public ActionResult Create()
        {
            List<Student> students = db.Student.ToList();
            ViewBag.students = students;

            return View();
        }

        // POST: Responsables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,address,phone_number,description,email")]
        Responsable responsable, List<int> students, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {
                if (students.Count > 0)
                {
                    System.Collections.IList listStudents = students;
                    for (int i = 0; i < listStudents.Count; i++)
                    {
                        string id = "" + listStudents[i];
                        Student student = db.Student.Find(id);
                        responsable.Student.Add(student);
                    }
                }





                db.Responsable.Add(responsable);
                db.SaveChanges();
                return RedirectToAction("/Index");
            }

            return View(responsable);
        }

        // GET: Responsables/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);
            if (responsable == null)
            {
                return HttpNotFound();
            }
            List<Student> students = responsable.Student.ToList();
            ViewBag.students = students;
            return View(responsable);
        }

        // POST: Responsables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,address,phone_number,description,email")] Responsable responsable, List<int> students)
        {
            if (ModelState.IsValid)
            {
                db.Entry(responsable).State = EntityState.Modified;

                Responsable responsab1e = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == responsable.identification);
                responsab1e.Student.Clear();
                if (students.Count > 0)
                {
                    System.Collections.IList listStudents = students;
                    for (int i = 0; i < listStudents.Count; i++)
                    {
                        string id = "" + listStudents[i];
                        Student student = db.Student.Find(id);
                        responsable.Student.Add(student);
                    }
                }



                db.SaveChanges();
                return RedirectToAction("/Index");
            }

            List<Student> studentsList = db.Student.ToList();
            ViewBag.students = studentsList;
            return View(responsable);
        }

        // GET: Responsables/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);

            List<Student> students = responsable.Student.ToList();
            ViewBag.students = students;

            if (responsable == null)
            {
                return HttpNotFound();
            }
            return View(responsable);
        }

        // POST: Responsables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);

            responsable.Student.Clear();


            db.Responsable.Remove(responsable);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("/Index");
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
