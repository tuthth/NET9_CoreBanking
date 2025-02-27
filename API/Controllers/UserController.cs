using API.CustomAttributes;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request.Create;
using Models.Request.Update;
using Models.Response;
using Services.Helpers;
using Services.Interfaces.UserManagement;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [JWTRequiredAtrribute]
    public class UserController : ControllerBase
    {
        private readonly ICreateUserService _createUserService;
        private readonly IUpdateUserService _updateUserService;
        private readonly IGetUserService _getUserService;
        private readonly IAccountService _accountService;

        public UserController(ICreateUserService createUserService, IUpdateUserService updateUserService, IGetUserService getUserService, IAccountService accountService)
        {
            _createUserService = createUserService;
            _updateUserService = updateUserService;
            _getUserService = getUserService;
            _accountService = accountService;
        }
        [EndpointSummary("Get a user by id")]
        [HttpGet("get/{id}")]
        public async Task<Result<UserResponse>> GetUserById(Guid id, CancellationToken cancellationToken) 
            => await _getUserService.GetUserById(id, cancellationToken);

        [EndpointSummary("Get users with pagination")]
        [HttpGet("get/page={page}/pageSize={pageSize}")]
        public async Task<Result<PaginatedResponse<UserResponse>>> GetUsersPagination(int page, int pageSize, CancellationToken cancellationToken)
            => await _getUserService.GetUsersPagination(page, pageSize, cancellationToken);

        [EndpointSummary("Login")]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<Result<string>> Login([FromBody] LoginRequest userLoginRequest, CancellationToken cancellationToken)
            => await _accountService.Login(userLoginRequest, cancellationToken);

        [EndpointSummary("Register")]
        [HttpPost("register")]
        public async Task<Result<string>> Register([FromBody] UserRegistration createUserRequest, CancellationToken cancellationToken)
            => await _createUserService.CreateNewUserTransaction(createUserRequest, cancellationToken);

        [EndpointSummary("Update user")]
        [HttpPatch("update")]
        public async Task<Result<string>> Update([FromBody] UserUpdateRequest updateUserRequest, CancellationToken cancellationToken)
            => await _updateUserService.UpdateUserTransaction(updateUserRequest, cancellationToken);

        [EndpointSummary("Restrict user from system")]
        [HttpPatch("restrict/{id}")]
        public async Task<Result> RestrictUser(Guid id, CancellationToken cancellationToken)
            => await _updateUserService.RestrictUser(id, cancellationToken);

        [EndpointSummary("Unrestrict user from system")]
        [HttpPatch("unrestrict/{id}")]
        public async Task<Result> UnrestrictUser(Guid id, CancellationToken cancellationToken)
            => await _updateUserService.RemoveRestrictForUser(id, cancellationToken);
    }
}
