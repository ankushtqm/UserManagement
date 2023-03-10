using A4A.UM.EntityDataModel;
using System.Collections.Generic;
using System.Linq;

namespace A4A.UM.Models
{
    /// <summary>
    /// Student data repository
    /// </summary>
    public class UserRepository
    {
        private static ATA_MembershipEntities _UserDb;

        private static ATA_MembershipEntities UserDb
        {
            get { return _UserDb ?? (_UserDb = new ATA_MembershipEntities()); }
        }

        /// <summary>
        /// Gets the students.
        /// </summary>
        /// <returns>IEnumerable Student List</returns>
        public static IEnumerable<User> GetAllUsers()
        {
            var query = from User in UserDb.Users
                        where User.IsActiveContact == true && User.UserId < 1000
                        select User;
            return query.ToList();
        }

        public static IEnumerable<User> GetA4AEMployees()
        {
            var query = from User in UserDb.Users
                        where User.IsATAStaff == true && User.IsActiveContact == true
                        select User;
            return query.ToList();
        }

        ///// <summary>
        ///// Inserts the student to database.
        ///// </summary>
        ///// <param name="student">The student object to insert.</param>
        //public static void InsertStudent(Student student)
        //{
        //    StudentDb.Students.Add(student);
        //    StudentDb.SaveChanges();
        //}

        /// <summary>
        /// Deletes student from database.
        /// </summary>
        /// <param name="studentId">Student ID</param>
        //public static void DeleteStudent(int studentId)
        //{
        //    var deleteItem = StudentDb.Students.FirstOrDefault(c => c.Id == studentId);

        //    if (deleteItem != null)
        //    {
        //        StudentDb.Students.Remove(deleteItem);
        //        StudentDb.SaveChanges();
        //    }
        //}
    }
}