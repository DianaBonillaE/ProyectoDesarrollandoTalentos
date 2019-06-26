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
        public ActionResult IndexEnrollment(int page = 1, int pageSize = 6)
        {
            List<Course> curseList = db.Course.ToList();
            PagedList<Course> model = new PagedList<Course>(curseList, page, pageSize);
            return View(model);
        }

        // GET: Curses/Enrollment
        public ActionResult ViewEnrollment(int? id, int page = 1, int pageSize = 6)
        {
            Course course = db.Course.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;

            List<Course_Student> courseStudentList = db.Course_Student.ToList();
            List<Student> mostrarCourseStudentList = new List<Student>();
            List<Teacher> teachers = course.Teacher.ToList();

            for (int i = 0; i < courseStudentList.Count; i++)
            {

                if (id.Equals(courseStudentList[i].curse_id))
                {
                    var idStudent = courseStudentList[i].student_identification;
                    Student student = db.Student.Find(idStudent);
                    mostrarCourseStudentList.Add(student);
                }
            }

            PagedList<Student> model = new PagedList<Student>(mostrarCourseStudentList, page, pageSize);
            ViewBag.students = mostrarCourseStudentList;
            ViewBag.teacher = teachers;
            ViewBag.startDate = course.start_date;
            ViewBag.endDate = course.end_date;

            return View(model);

        }

        public ActionResult CreateEnrollment(int? id, int page = 1, int pageSize = 6)
        {
            Course course = db.Course.Find(id);
            ViewBag.name = course.name;
            ViewBag.id = course.id;
            ViewBag.fechaFinalizacion = course.end_date;

            List<Course_Student> courseStudentList = db.Course_Student.ToList();
            List<Student> enrollmentCourseStudentList = new List<Student>();
            List<Student> noEnrollmentCourseStudentList = new List<Student>();
            List<Student> students = db.Student.ToList();

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

            PagedList<Student> model = new PagedList<Student>(noEnrollmentCourseStudentList, page, pageSize);
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
            List<Course_Student> enrollments = course.Course_Student.ToList();

            ViewBag.studentId = student.identification;
            ViewBag.studentName = student.name;
            ViewBag.studentLastName = student.last_name;

            ViewBag.curseName = course.name;

            for (int i=0; i < enrollments.Count(); i++)
            {
                Course_Student enrollment = enrollments[i];
                if(enrollment.curse_id == idCourse && enrollment.student_identification.Equals(idStudent))
                {
                    db.Course_Student.Remove(enrollment);
                }
            }

            db.SaveChanges();

            return View();
        }
    }
}