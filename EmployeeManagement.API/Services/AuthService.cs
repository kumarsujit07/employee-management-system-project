using System.Threading.Tasks;
using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Helpers;
using EmployeeManagement.API.Interfaces;

namespace EmployeeManagement.API.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public class InvalidCredentialsException : System.Exception
        {
            public InvalidCredentialsException(string message) : base(message) { }
        }

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            // Plain text comparison based on comment feedback
            if (user == null || user.Password != request.Password)
            {
                throw new InvalidCredentialsException("Invalid username or password.");
            }

            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                EmployeeId = user.EmployeeId
            };
        }
    }
}
