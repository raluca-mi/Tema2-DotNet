using Core.Dtos;
using DataLayer;
using DataLayer.Entities;

namespace Core.Services
{
    public class UserService
    {
        private readonly UnitOfWork unitOfWork;
        private AuthorizationService authService { get; set; }

        public UserService(UnitOfWork unitOfWork, AuthorizationService authService)
        {
            this.unitOfWork = unitOfWork;
            this.authService = authService;
        }

        public void Register(RegisterDto registerData)
        {
            if (registerData == null)
            {
                return;
            }

            var hashedPassword = authService.HashPassword(registerData.Password);

            var user = new User
            {
                Email = registerData.Email,
                Password = hashedPassword,
                RoleId = registerData.RoleId, 
            };

            unitOfWork.Users.Insert(user);
            unitOfWork.SaveChanges();
        }

        public string Validate(LoginDto payload)
        {
            var user = unitOfWork.Users.GetByEmail(payload.Email);

            var passwordFine = authService.VerifyPassword(payload.Password, user.Password);

            if (passwordFine)
            {
                return authService.GetToken(user);
            }
            else
            {
                return null;
            }

        }

    }
}
