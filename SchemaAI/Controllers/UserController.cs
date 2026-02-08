using CommonEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchemaAI.Entities;
using SchemaAI.ServiceContract;
using SchemaAI.Services;

namespace SchemaAI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }


        [HttpGet("by-User/{Userguid:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetByUser(Guid Userguid)
        {
            var Users = await _service.GetAsync(Userguid);
            return Ok(Users);
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var Users = await _service.GetAllAsync();
            return Ok(Users);
        }

        // POST: api/Users
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] User User)
        {
            if (User == null)
                return BadRequest("User payload is required.");

            var createdUser = await _service.CreateAsync(User);

            return CreatedAtAction(
                nameof(GetByUser),
                new { UserGuid = createdUser.UserGuid },
                createdUser
            );
        }

        // PUT: api/Users/{UserGuid}
        [HttpPut("{UserGuid:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> Update(Guid UserGuid, [FromBody] User User)
        {
            if (User == null)
                return BadRequest("User payload is required.");

            if (User.UserGuid != Guid.Empty && User.UserGuid != UserGuid)
                return BadRequest("UserGuid in route and body do not match.");

            User.UserGuid = UserGuid;

            await _service.UpdateAsync(User);

            return CreatedAtAction(
               nameof(GetByUser),
               new { UserGuid = UserGuid },
               User
           );
         
        }

        // DELETE: api/Users/{UserGuid}
        [HttpDelete("{UserGuid:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        public async Task<IActionResult> Delete(Guid UserGuid)
        {
            await _service.DeleteAsync(UserGuid);
            return NoContent();
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(User objUser)
        {
            await _service.ChangePassword(objUser);
            return Ok("Your password has been reset successfully. If you did not request this change, please contact our support team immediately.");
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
        [FromQuery] PagingRequest request)
        {
            var result = await _service.GetPagedAsync(
                request.PageNumber,
                request.PageSize);

            return Ok(result);
        }

        [HttpGet("scrollPaged")]
        public async Task<IActionResult> GetScrollAsync(
        [FromQuery] ScrolledPagingRequest request)
        {
            var result = await _service.GetScrollAsync(
                request.LastId,
                request.PageSize);

            return Ok(result);
        }
    }
}
