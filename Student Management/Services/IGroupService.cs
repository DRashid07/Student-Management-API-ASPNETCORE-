using Student_Management.DTO.GroupDto;
using Student_Management.Models;
using System.Collections.Generic;

namespace Student_Management.Services
{
    public interface IGroupService
    {
        IEnumerable<GetGroupDto> GetAllGroups();
        GetGroupDto GetGroupById(int id);
        bool AddGroup(CreateGoupDto groupDto, out string errorMessage);
        bool UpdateGroup(int id, UpdateGroupDto updatedGroupDto, out string errorMessage);
        bool DeleteGroup(int id);
    }
}