using Student_Management.Data;
using Student_Management.Models;
using Student_Management.Repositories.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Student_Management.Repositories.Concrete
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student GetById(int id)
        {
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }

        public Student GetByEmail(string email)
        {
            return _context.Students.FirstOrDefault(s => s.Email == email);
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }
    }
}