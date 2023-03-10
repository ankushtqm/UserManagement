using A4A.UM.EntityDataModel;
using A4A.UM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace A4A.UM.Controllers
{
    /// <summary>
    /// Student Api controller
    /// </summary>
    public class StudentController : ApiController
    {
        // GET api/student
        public IEnumerable<Student> Get()
        {
            return StudentRepository.GetStudents();
        }

        // GET api/student/5
        public Student Get(int id)
        {
            return StudentRepository.GetStudents().FirstOrDefault(s=>s.Id == id);
        }

        // POST api/student
        public HttpResponseMessage Post(Student student)
        {
            StudentRepository.InsertStudent(student);
            var response = Request.CreateResponse(HttpStatusCode.Created, student);
            string url = Url.Link("DefaultApi", new {student.Id});
            response.Headers.Location = new Uri(url);

            return response;
        }

        // DELETE api/student/5
        public HttpResponseMessage Delete(int id)
        {
            StudentRepository.DeleteStudent(id);
            var response = Request.CreateResponse(HttpStatusCode.OK, id);
            return response;
        }
    }
}
