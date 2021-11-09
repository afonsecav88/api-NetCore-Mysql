
using mysqlapi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mysqlapi.Interfaces
{
    public interface IStudentDetail
    {
        Task<IEnumerable<StudentDetail>> GetStudentDetails();
        Task<StudentDetail> GetStudentDetailById(int id);
        Task<bool> CreateStudentDetail(StudentDetail studentDetail);
        Task<bool> DeleteStudentDetail(StudentDetail studentDetail);
        Task<bool> UpdateStudentDetail(StudentDetail studentDetail, int id);
        Task<bool> StudentDetailExists(int id);
    }
}
