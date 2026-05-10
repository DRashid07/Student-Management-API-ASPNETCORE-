using Microsoft.AspNetCore.Mvc;
using Student_Management.DTO.GroupDto;
using Student_Management.Services;

namespace Student_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("GetAllGroups")]
        public IActionResult GetAllGroups()
        {
            var groups = _groupService.GetAllGroups();
            return Ok(groups);
        }

        [HttpGet("GetGroupById/{id}")]
        public IActionResult GetGroupById(int id)
        {
            var group = _groupService.GetGroupById(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }

        [HttpPost("AddGroup")]
        public IActionResult AddGroup([FromBody] CreateGoupDto groupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_groupService.AddGroup(groupDto, out string errorMessage))
            {
                ModelState.AddModelError("Name", errorMessage);
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Group created successfully" });
        }

        [HttpPut("UpdateGroup/{id}")]
        public IActionResult UpdateGroup(int id, [FromBody] UpdateGroupDto updatedGroupDto)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = _groupService.GetGroupById(id);
            if (group == null)
            {
                return NotFound();
            }

            if (!_groupService.UpdateGroup(id, updatedGroupDto, out string errorMessage))
            {
                 ModelState.AddModelError("Name", errorMessage);
                 return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpDelete("DeleteGroup/{id}")]
        public IActionResult DeleteGroup(int id)
        {
            if (!_groupService.DeleteGroup(id))
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
