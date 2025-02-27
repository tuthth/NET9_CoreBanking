using API.CustomAttributes;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request.Create;
using Models.Response;
using Services.Helpers;
using Services.Interfaces.BankAccountManagement;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [JWTRequiredAtrribute]
    public class BankAccountController : ControllerBase
    {
        private readonly ICreateBankAccountService _createBankAccountService;
        private readonly IGetBankAccountService _getBankAccountService;

        public BankAccountController(ICreateBankAccountService createBankAccountService, IGetBankAccountService getBankAccountService)
        {
            _createBankAccountService = createBankAccountService;
            _getBankAccountService = getBankAccountService;
        }
        [EndpointSummary("Create a new bank account")]
        [HttpPost("create")]
        public async Task<Result<string>> CreateBankAccount([FromBody] BankAccountRegistration bankAccount, CancellationToken cancellationToken)
            => await _createBankAccountService.CreateBankAccount(bankAccount, cancellationToken);

        [EndpointSummary("Get a bank account by id")]
        [HttpGet("get/{id}")]
        public async Task<Result<BankAccountResponse>> GetBankAccountById(Guid id, CancellationToken cancellationToken)
            => await _getBankAccountService.GetBankAccountById(id, cancellationToken);

        [EndpointSummary("Get bank accounts with pagination")]
        [HttpGet("get/page={page}/pageSize={pageSize}")]
        public async Task<Result<PaginatedResponse<BankAccountResponse>>> GetBankAccountsPagination(int page, int pageSize, CancellationToken cancellationToken)
            => await _getBankAccountService.GetBankAccountsPagination(page, pageSize, cancellationToken);
    }
}
