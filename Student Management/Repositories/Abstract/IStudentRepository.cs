using Student_Management.Models;
using System.Collections.Generic;

namespace Student_Management.Repositories.Abstract
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student GetById(int id);
        Student GetByEmail(string email);
        void Add(Student student);
        void Update(Student student);
        void Delete(int id);
    }
}