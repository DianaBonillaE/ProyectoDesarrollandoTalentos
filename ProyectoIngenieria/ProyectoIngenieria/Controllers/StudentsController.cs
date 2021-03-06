﻿using System;
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
    public class StudentsController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Students
        public ActionResult Index(string message, int page = 1, int pageSize = 5)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }

            List<Student> studentList = db.Student.ToList();
            PagedList<Student> model = new PagedList<Student>(studentList, page, pageSize);
            return View(model);


            var student = db.Student.Include(s => s.Responsable);
            return View(student.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        public ActionResult Responsable(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            Responsable responsable = db.Responsable.Find(student.responsable_identification);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(responsable);
        }


        // GET: Students/Create
        public ActionResult Create(string message)
        {
            if (message != null)
            {
                ViewBag.message = message;
            }
            ViewBag.responsable_identification = new SelectList(db.Responsable, "identification", "identification");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identification,name,last_name,phone_number,responsable_identification")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Student.Add(student);


              
            }

            ViewBag.responsable_identification = new SelectList(db.Responsable, "identification", "identification", student.responsable_identification);
            
                try
                {
                    db.SaveChanges();
                return RedirectToAction("Index", new { message = "El estudiante se ingresó exitosamente" });
            }
                catch (Exception e)
                {
                ViewBag.message = "Ya existe un estudiante con esta Identificación";
                    return View();
                }
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.responsable_identification = new SelectList(db.Responsable, "identification", "identification", student.responsable_identification);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identification,name,last_name,phone_number,responsable_identification")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { message = "El estudiante se actualizo exitosamente" });
            }
            ViewBag.responsable_identification = new SelectList(db.Responsable, "identification", "identification", student.responsable_identification);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Student.Find(id);

            Course_Student curse_Student = db.Course_Student.Include(a => a.Student).ToList().Find(c => c.student_identification == id);
            if (curse_Student != null) {
                db.Course_Student.Remove(curse_Student);
            }

           
            db.Student.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "El estudiante se eliminó exitosamente" });
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
