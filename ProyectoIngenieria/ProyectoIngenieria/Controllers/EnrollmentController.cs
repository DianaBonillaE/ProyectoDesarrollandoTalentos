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
            List<Course> curseList = db.Course.ToList();
            PagedList<Course> model = new PagedList<Course>(curseList, page, pageSize);
            return View(model);
        }

        // GET: Curses/Enrollment
        public ActionResult ViewEnrollment(int? id, int page = 1, int pageSize = 4)
        {
            Course course = db.Course.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;

            List<Course_Student> courseStudentList = db.Course_Student.ToList();
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
            Course course = db.Course.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;
            ViewBag.fechaFinalizacion = course.end_date;

            List<Course_Student> courseStudentList = db.Course_Student.ToList();
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
            Course_Student enrollment = new Course_Student();
            enrollment.curse_id = idCourse;
            enrollment.student_identification = idStudent;

            db.Course_Student.Add(enrollment);

            Student student = db.Student.Find(idStudent);
            student.Course_Student.Add(enrollment);

            Course course = db.Course.Find(idCourse);

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
            Course course = db.Course.Find(idCourse);

            ViewBag.studentId = student.identification;
            ViewBag.studentName = student.name;
            ViewBag.studentLastName = student.last_name;

            ViewBag.curseName = course.name;

            List<Course_Student> courseStudentList = db.Course_Student.ToList();

            Course_Student curse_student = new Course_Student();

            for (int i = 0; i < courseStudentList.Count; i++)
            {
                if (courseStudentList[i].curse_id.Equals(idCourse) && courseStudentList[i].student_identification.Equals(idStudent))
                {
                    var id = courseStudentList[i].id;

                  //  curse_student = db.Curse_Student.Find((idStudent)keyVa, idCourse);

                }
            }
            

            db.Course_Student.Remove(curse_student);
            db.SaveChanges();

            return View();
        }
    }
}