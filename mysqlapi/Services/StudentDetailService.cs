using Microsoft.EntityFrameworkCore;
using mysqlapi.Interfaces;
using mysqlapi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mysqlapi.Services
{
    public class StudentDetailService : IStudentDetail
    {
        private readonly StudentDetailContext _context;
        public StudentDetailService(StudentDetailContext context)
        {
            _context = context;
        }

        public async Task<StudentDetail> GetStudentDetailById(int id)
        {
            return await _context.StudentDetails.FindAsync(id);
        }

        public async Task<IEnumerable<StudentDetail>> GetStudentDetails()
        {
            return await _context.StudentDetails.ToListAsync();
        }

        public async Task<bool> CreateStudentDetail(StudentDetail studentDetail)
        {
            await _context.StudentDetails.AddAsync(studentDetail);
            return await Save();
        }

        public async Task<bool> DeleteStudentDetail(StudentDetail studentDetail)
        {
            _context.StudentDetails.Remove(studentDetail);
            return await Save();
            
        }

        public async Task<bool> StudentDetailExists(int id)
        {
            return await _context.StudentDetails.AnyAsync(e => e.Id == id);
        }

        private async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0 ? true : false;
        }

        public async Task<bool> UpdateStudentDetail(StudentDetail studentDetail, int id)
        {          
            _context.Entry(studentDetail).State = EntityState.Modified;

            return await Save();
        }
    }
}
