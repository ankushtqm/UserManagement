using A4A.UM.EntityDataModel;
using System.Collections.Generic;
using System.Linq;

namespace A4A.UM.Models
{
    public class GroupRepository
    {
        private static ATA_MembershipEntities _A4AMemDb;

        private static ATA_MembershipEntities A4AMembershipDb
        {
            get { return _A4AMemDb ?? (_A4AMemDb = new ATA_MembershipEntities()); }
        }


        /// <summary>
        /// Gets All the Groups.
        /// </summary>
        /// <returns>IEnumerable Student List</returns>
        public static IEnumerable<Group> GetAllGroups()
        {
            var query = from Group in A4AMembershipDb.Groups 
                        select Group;
            return query.ToList();
        }

        /// <summary>
        /// Gets All the security Groups.
        /// </summary>
        /// <returns>IEnumerable Student List</returns>
        public static IEnumerable<Group> GetAllSecurityGroups()
        {
            var query = from Group in A4AMembershipDb.Groups
                        where Group.IsSecurityGroup == true
                        select Group;
            return query.ToList();
        }

        /// <summary>
        /// Gets All the distribution Groups.
        /// </summary>
        /// <returns>IEnumerable Student List</returns>
        public static IEnumerable<Group> GetAllDistributionGroups()
        {
            var query = from Group in A4AMembershipDb.Groups
                        where Group.IsSecurityGroup == false  && Group.IsCommittee == false
                        select Group;
            return query.ToList();
        }

        /// <summary>
        /// Gets All theCouncil & Committies Groups.
        /// </summary>
        /// <returns>IEnumerable Student List</returns>
        public static IEnumerable<Group> GetAllCouncilCommittiesGroups()
        {
            var query = from Group in A4AMembershipDb.Groups
                        where  Group.IsCommittee == true
                        select Group;
            return query.ToList();
        }


        /// <summary>
        /// Inserts the student to database.
        /// </summary>
        /// <param name="student">The student object to insert.</param>
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