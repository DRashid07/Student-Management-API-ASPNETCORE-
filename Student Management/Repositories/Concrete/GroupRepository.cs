using Student_Management.Data;
using Student_Management.Models;
using Student_Management.Repositories.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Student_Management.Repositories.Concrete
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Group> GetAll()
        {
            return _context.Groups.ToList();
        }

        public Group GetById(int id)
        {
            return _context.Groups.FirstOrDefault(g => g.Id == id);
        }

        public Group GetByName(string name)
        {
            return _context.Groups.FirstOrDefault(g => g.Name == name);
        }

        public void Add(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        public void Update(Group group)
        {
            _context.Groups.Update(group);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id)
        {
            return _context.Groups.Any(g => g.Id == id);
        }
    }
}