using Student_Management.DTO.GroupDto;
using Student_Management.Models;


namespace Student_Management.Services
{
    public class GroupService : IGroupService
    {
   
        private static readonly List<Group> _groups = new List<Group>
        {
            new Group { Id = 1, Name = "Group A" },
            new Group { Id = 2, Name = "Group B" }
        };

        public IEnumerable<GetGroupDto> GetAllGroups()
        {
            return _groups.Select(g => new GetGroupDto
            {
                Id = g.Id,
                Name = g.Name
            });
        }

        public GetGroupDto GetGroupById(int id)
        {
            var group = _groups.FirstOrDefault(g => g.Id == id);
            if (group == null) return null;

            return new GetGroupDto
            {
                Id = group.Id,
                Name = group.Name
            };
        }

        public bool AddGroup(CreateGoupDto groupDto, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_groups.Any(g => g.Name == groupDto.Name))
            {
                errorMessage = "Name unikal olmalıdır.";
                return false;
            }

            var group = new Group
            {
                Id = _groups.Count > 0 ? _groups.Max(g => g.Id) + 1 : 1,
                Name = groupDto.Name
            };

            _groups.Add(group);
            return true;
        }

        public bool UpdateGroup(int id, UpdateGroupDto updatedGroupDto, out string errorMessage)
        {
            errorMessage = string.Empty;

            var group = _groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
            {
                return false; 
            }

            if (_groups.Any(g => g.Name == updatedGroupDto.Name && g.Id != id))
            {
                errorMessage = "Name unikal olmalıdır.";
                return false;
            }

            group.Name = updatedGroupDto.Name;
            return true;
        }

        public bool DeleteGroup(int id)
        {
            var group = _groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
            {
                return false;
            }
            _groups.Remove(group);
            return true;
        }

     
        public static bool GroupExists(int id)
        {
             return _groups.Any(g => g.Id == id);
        }
    }
}