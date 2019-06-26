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

        private static bool stateCreate;
        private static bool deleteWarning;
        private static string idResponsable;

        // GET: Responsables
        public ActionResult Index(string message, int page = 1, int pageSize = 5)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
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
            stateCreate = true;
            return View();
        }

        // POST: Responsables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,address,phone_number,description,email")]
        Responsable responsable, [Bind(Include = "identification,name,last_name,phone_number,responsable_identification")]Student student, List<int> students, HttpPostedFileBase File, string nameFile)
        {
            if (ModelState.IsValid)
            {

                db.Responsable.Add(responsable);
                try
                {
                    db.SaveChanges();
                   }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "ERROR! en igresar un responsable ya existe un responsable con esta Identificación");
                    if (stateCreate) {
                    return View(); }
                    else {
                        return View("CreateResponsable");
                    }
                }

                if (stateCreate) { 
                student.responsable_identification = responsable.identification;
                db.Student.Add(student);
                stateCreate = false;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    db.Student.Remove(student);
                    db.Responsable.Remove(responsable);
                    stateCreate = true;
                    db.SaveChanges();
                    ModelState.AddModelError("", "ERROR! en igresar un estudiante ya existe un estudiante con esta Identificación");
                    return View();
                }




                return RedirectToAction("Index", new { message = "El responsable y estudiante se ingresaron exitosamente" });
            }

            return View(responsable);
        }

        // GET: Responsables/Create
        public ActionResult CreateResponsable()
        {
            stateCreate = false;

            return View();
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

                
                


                db.SaveChanges();
                return RedirectToAction("Index", new { message = "Se actualizó el responsable exitosamente" });
            }

          
            return View(responsable);
        }

        // GET: Responsables/Delete
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

            if (students.Count >0)
            { return RedirectToAction("/DeleteWarning/"+id); }
            else { return View(responsable); }


           
        }

        // POST: Responsables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);

           

            db.Responsable.Remove(responsable);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index", new { message = "El responsable se eliminó exitosamente" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult DeleteWarning(string id, int page = 1, int pageSize = 5){
            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == id);

            List<Student> studentList = responsable.Student.ToList();
            deleteWarning = true;
            idResponsable = id;

            PagedList<Student> model = new PagedList<Student>(studentList, page, pageSize);
            return View(model);
        }

        
        [HttpPost, ActionName("DeleteWarning")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWarning()
        {

            Responsable responsable = db.Responsable.Include(a => a.Student).ToList().Find(c => c.identification == idResponsable);
            
            List<Student> studentList = responsable.Student.ToList();
            
            
            for (int i = 0; i < studentList.Count; i++)
            {
                string idStudent = studentList[i].identification;
                Student student = db.Student.Find(idStudent);

                Course_Student curse_Student = db.Course_Student.Include(a => a.Student).ToList().Find(c => c.student_identification == idStudent);
                if (curse_Student != null)
                {
                    db.Course_Student.Remove(curse_Student);
                }


                db.Student.Remove(student);
                db.SaveChanges();
            }
            responsable.Student.Clear();
            db.Responsable.Remove(responsable);

            
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index", new { message = "El responsable y los estudiantes se eliminaron exitosamente" });
        }

    }
}
