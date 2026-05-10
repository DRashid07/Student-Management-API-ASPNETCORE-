using Student_Management.Models;
using System.Collections.Generic;

namespace Student_Management.Repositories.Abstract
{
    public interface IGroupRepository
    {
        IEnumerable<Group> GetAll();
        Group GetById(int id);
        Group GetByName(string name);
        void Add(Group group);
        void Update(Group group);
        void Delete(int id);
        bool Exists(int id);
    }
}