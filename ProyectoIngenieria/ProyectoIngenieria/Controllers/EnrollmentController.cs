using PagedList;
using ProyectoIngenieria.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoIngenieria.Controllers
{
    public class EnrollmentController : Controller
    {
        private ProyectoIngenieriaEntities db = new ProyectoIngenieriaEntities();

        // GET: Curses
        public ActionResult IndexEnrollment(int page = 1, int pageSize = 4)
        {
            List<Curse> curseList = db.Curse.ToList();
            PagedList<Curse> model = new PagedList<Curse>(curseList, page, pageSize);
            return View(model);
        }

        // GET: Curses/Enrollment
        public ActionResult ViewEnrollment(int? id)
        {
            Curse course = db.Curse.Find(id);
            ViewBag.name = course.name;

            List<Curse_Student> courseStudentList = db.Curse_Student.ToList();
            List<Student> mostrarCourseStudentList = new List<Student>();

            for (int i = 0; i < courseStudentList.Count; i++)
            {

                if (id.Equals(courseStudentList[i].curse_id))
                {
                    var idStudent = courseStudentList[i].student_identification;
                    Student student = db.Student.Find(idStudent);
                    mostrarCourseStudentList.Add(student);
                }
            }

            ViewBag.students = mostrarCourseStudentList;

            return View();

        }

        public ActionResult CreateEnrollment(int? id)
        {
            Curse course = db.Curse.Find(id);
            ViewBag.name = course.name;
            ViewBag.fechaFinalizacion = course.end_date;

            List<Curse_Student> courseStudentList = db.Curse_Student.ToList();
            List<Student> mostrarCourseStudentList = new List<Student>();

            for (int i = 0; i < courseStudentList.Count; i++)
            {

                if (id != courseStudentList[i].curse_id)
                {
                    var idStudent = courseStudentList[i].student_identification;
                    Student student = db.Student.Find(idStudent);
                    mostrarCourseStudentList.Add(student);
                }
            }

            ViewBag.students = mostrarCourseStudentList;

            return View();
        }
    }
}