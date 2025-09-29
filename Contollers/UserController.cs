using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Last_Api.Models;
using Microsoft.AspNetCore.Authorization;
namespace Last_Api.Contollers
{
    [ApiController]
    [Route("api/user")]
    [Tags("Artifact Endpoints:")]
    public class UserContoller : ControllerBase
    {
        private readonly IUser _UserRepo;

        public UserContoller(IUser UserRepo)
        {
            _UserRepo = UserRepo;
        }

        [Authorize(Policy = "admin")]
        [HttpGet()]
        [EndpointName("Get Users")]
        [EndpointSummary("Returns all Users")]
        [EndpointDescription("Authorization limited to Admin. Returns a list containing objects of User type")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<User>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_UserRepo.GetUsers());
        }

        [Authorize(Policy = "admin")]
        [HttpGet("{email}")]
        [EndpointName("Get User")]
        [EndpointSummary("Returns all User")]
        [EndpointDescription("Authorization limited to Admin. Returns an object of User type matching email given. Example Request: /api/user/example@email.com")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<User>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<User> GetUser(string email)
        {
            var user = _UserRepo.GetUser(email);
            if (user == null)
            {
                return NotFound($"No user with the email: {email} found");
            }
            return Ok(user);
        }

        [Authorize(Policy = "admin")]
        [HttpPost()]
        [EndpointName("Add User")]
        [EndpointSummary("Add User")]
        [EndpointDescription(@"Authorization limited to Admins. Adds new User to the database.
        Body should look like:
        {
        ""email"": ""EMAIL@EMAIL.COM"",
        ""password"": ""PASSWORD"",
        ""role"": ""ROLE""
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<User> AddUser(User newUser)
        {
            var _user = _UserRepo.GetUser(newUser.email);
            if (_user != null) { return NotFound($"User with email '{newUser.email}' already exists"); }
            if (newUser.email == null) { return BadRequest("email cannot be null"); }
            if (newUser.password == null) { return BadRequest("password cannot be null"); }
            if (!newUser.email.Contains('@')) { return BadRequest("Email is not a valid email address"); }
            newUser.role.ToLower();
            var result = _UserRepo.AddUser(newUser);
            return Created();
        }

        [Authorize(Policy = "admin")]
        [HttpPatch("email/{oldEmail}")]
        [EndpointName("Patch email")]
        [EndpointSummary("Patch email")]
        [EndpointDescription(@"Authorization limited to Admins. Updates existing users email.
        Request should look like: /api/user/email/Bobby@Gmail.com
        Body should look like:
        {
        ""email"": ""EMAIL@EMAIL.COM"",
        ""password"": ""NULL"",
        ""role"": ""NULL""
        }
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> PatchEmail(string oldEmail, User user)
        {
            Console.WriteLine($"Email: {oldEmail}");

            if (user.email == null) { return BadRequest("New Email cannot be null"); }
            if (!user.email.Contains('@')) { return BadRequest("New Email is not a valid email address"); }

            if (user.role.Count() > 0) { return BadRequest("Role should be empty, if attempting to update role please us PatchRole"); }
            if (user.password.Count() > 0) { return BadRequest("Password should be empty, if attempting to update password please us PatchPassword"); }

            var _user = _UserRepo.GetUser(oldEmail);
            if (_user == null) { return NotFound($"No user with email '{oldEmail}' found"); }

            _user.email = user.email;
            _UserRepo.PatchEmail(oldEmail, _user);
            return NoContent();
        }

        [Authorize(Policy = "admin")]
        [HttpPatch("password")]
        [EndpointName("Patch password")]
        [EndpointSummary("Patch Password")]
        [EndpointDescription(@"Authorization limited to Admins. Updates existing users password.
        Request should look like: /api/user/password
        Body should look like:
        {
        ""email"": ""EMAIL@EMAIL.COM"",
        ""password"": ""NEW PASSWORD"",
        ""role"": ""NULL""
        }
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> PatchPassword(User user)
        {
            if (user.role.Count() > 0) { return BadRequest("Role should be empty, if attempting to update role please us PatchRole"); }
            if (user.password == null) { return BadRequest("New Password cannot be null"); }

            var _user = _UserRepo.GetUser(user.email);
            if (_user == null) { return NotFound($"No user with email '{user.email}' found"); }

            _user.password = user.password;
            _UserRepo.PatchPassword(_user);
            return NoContent();
        }


        [Authorize(Policy = "admin")]
        [HttpPatch("role")]
        [EndpointName("Patch role")]
        [EndpointSummary("Patch role")]
        [EndpointDescription(@"Authorization limited to Admins. Updates existing users role.
        Request should look like: /api/user/role
        Body should look like:
        {
        ""email"": ""EMAIL@EMAIL.COM"",
        ""password"": ""NULL"",
        ""role"": ""NEW ROLE""
        }
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> PatchRole(User user)
        {
            if (user.password.Count() > 0) { return BadRequest("Password should be empty, if attempting to update password please us PatchPassword"); }
            if (user.role == null) { return BadRequest("New Role cannot be null"); }

            var _user = _UserRepo.GetUser(user.email);
            if (_user == null) { return NotFound($"No user with email '{user.email}' found"); }

            _user.role = user.role;
            _UserRepo.PatchRole(_user);
            return NoContent();
        }


        [Authorize(Policy = "admin")]
        [HttpDelete("{email}")]
        [EndpointName("Deletes user")]
        [EndpointSummary("Deletes user from database")]
        [EndpointDescription(@"Authorization limited to Admins. Deletes User.
        Example Request: /api/user/email@google.com
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteUser(string email)
        {
            var user = _UserRepo.GetUser(email);
            if (user == null) { return NotFound($"No user with the email: {email} found"); }
            _UserRepo.DeleteUser(email);
            return NoContent();
        }


    }
}
