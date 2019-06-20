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
        public ActionResult ViewEnrollment(int? id, int page = 1, int pageSize = 4)
        {
            Curse course = db.Curse.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;

            List<Curse_Student> courseStudentList = db.Curse_Student.ToList();
            List<Student> mostrarCourseStudentList = new List<Student>();

            PagedList<Student> model = new PagedList<Student>(mostrarCourseStudentList, page, pageSize);

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

            return View(model);

        }

        public ActionResult CreateEnrollment(int? id, int page = 1, int pageSize = 4)
        {
            Curse course = db.Curse.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;
            ViewBag.fechaFinalizacion = course.end_date;

            List<Curse_Student> courseStudentList = db.Curse_Student.ToList();
            List<Student> enrollmentCourseStudentList = new List<Student>();
            List<Student> noEnrollmentCourseStudentList = new List<Student>();
            List<Student> students = db.Student.ToList();

            PagedList<Student> model = new PagedList<Student>(noEnrollmentCourseStudentList, page, pageSize);

            //Estudiantes matriculados
            for (int i = 0; i < courseStudentList.Count; i++)
            {

                if (id.Equals(courseStudentList[i].curse_id))
                {
                    var idStudent = courseStudentList[i].student_identification;
                    Student student = db.Student.Find(idStudent);
                    enrollmentCourseStudentList.Add(student);
                }
            }

            //Estudiantes sin matricular
            for (int i = 0; i < students.Count; i++)
            {
                Student student = db.Student.Find(students[i].identification);
                if (!enrollmentCourseStudentList.Contains(student))
                {
                    noEnrollmentCourseStudentList.Add(student);
                }
            }

            ViewBag.students = noEnrollmentCourseStudentList;

            return View(model);
        }

        public ActionResult ConfirmEnrollment(string idStudent, int idCourse)
        {
            Curse_Student enrollment = new Curse_Student();
            enrollment.curse_id = idCourse;
            enrollment.student_identification = idStudent;

            db.Curse_Student.Add(enrollment);

            Student student = db.Student.Find(idStudent);
            student.Curse_Student.Add(enrollment);

            Curse course = db.Curse.Find(idCourse);

            db.SaveChanges();

            ViewBag.studentId = student.identification;
            ViewBag.studentName = student.name;
            ViewBag.studentLastName = student.last_name;

            ViewBag.curseName = course.name;

            return View();
        }
        public ActionResult DeleteEnrollment(string idStudent, int idCourse)
        {

            Student student = db.Student.Find(idStudent);
            Curse course = db.Curse.Find(idCourse);

            ViewBag.studentId = student.identification;
            ViewBag.studentName = student.name;
            ViewBag.studentLastName = student.last_name;

            ViewBag.curseName = course.name;

            List<Curse_Student> courseStudentList = db.Curse_Student.ToList();

            Curse_Student curse_student = new Curse_Student();

            for (int i = 0; i < courseStudentList.Count; i++)
            {
                if (courseStudentList[i].curse_id.Equals(idCourse) && courseStudentList[i].student_identification.Equals(idStudent))
                {
                    var id = courseStudentList[i].id;

                  //  curse_student = db.Curse_Student.Find((idStudent)keyVa, idCourse);

                }
            }
            

            db.Curse_Student.Remove(curse_student);
            db.SaveChanges();

            return View();
        }
    }
}