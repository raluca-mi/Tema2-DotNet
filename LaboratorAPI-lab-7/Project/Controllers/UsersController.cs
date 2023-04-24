using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private UserService userService{ get; set; }
        private StudentService studentService { get; set; }

        public UsersController(UserService userService, StudentService studentService)
        {
            this.userService = userService;
            this.studentService = studentService;
        }

        [HttpPost("/register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterDto payload)
        {
            userService.Register(payload);
            return Ok();
        }

        [HttpPost("/login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDto payload)
        {
            var jwtToken = userService.Validate(payload);
            return Ok(new { token = jwtToken });
        }

        [HttpGet("students-only")]
        [Authorize(Roles = "Student")]
        public ActionResult<string> HelloStudents()
        {
            return Ok("Hello students!");
        }

        [HttpGet("teacher-only")]
        [Authorize(Roles = "Teacher")]
        public ActionResult<string> HelloTeachers()
        {
            return Ok("Hello teachers!");
        }

        [HttpGet("get-grades")]
        [Authorize(Roles = "Student, Teacher")]

        public IActionResult GetGrades()
        {
            ClaimsPrincipal user = User;


            if (user.IsInRole("Student"))
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;


                var student = studentService.GetByUserId(int.Parse(userId));
                var myGrades = student.Grades.ToList();

                return Ok(myGrades);
            }
            else if (user.IsInRole("Teacher"))
            {
                var result = studentService.GetGradesByAllStudents();

                return Ok(result);
            }
            return Ok();

        }
    }
}
