using API.CustomAttributes;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Request.Create;
using Models.Response;
using Services.Helpers;
using Services.Interfaces.TransactionManagement;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [JWTRequiredAtrribute]
    public class TransactionController : ControllerBase
    {
        private readonly ICreateTransactionService _createTransactionService;
        private readonly IGetTransactionService _getTransactionService;
        private readonly IUpdateTransactionService _updateTransactionService;

        public TransactionController(ICreateTransactionService createTransactionService, IGetTransactionService getTransactionService, IUpdateTransactionService updateTransactionService)
        {
            _createTransactionService = createTransactionService;
            _getTransactionService = getTransactionService;
            _updateTransactionService = updateTransactionService;
        }

        [EndpointSummary("Create a new transaction")]
        [HttpPost("create")]
        public async Task<Result> CreateTransaction([FromBody] TransactionRequest transaction, CancellationToken cancellationToken)
            => await _createTransactionService.CreateTransaction(transaction, cancellationToken);

        [EndpointSummary("Get a transaction by id")]
        [HttpGet("get/{id}")]
        public async Task<Result<TransactionResponse>> GetTransactionById(Guid id, CancellationToken cancellationToken)
            => await _getTransactionService.GetTransactionById(id, cancellationToken);

        [EndpointSummary("Get transactions with pagination")]
        [HttpGet("get/page={page}/pageSize={pageSize}")]
        public async Task<Result<PaginatedResponse<TransactionResponse>>> GetTransactionsPagination(int page, int pageSize, CancellationToken cancellationToken)
            => await _getTransactionService.GetTransactions(page, pageSize, cancellationToken);

        [EndpointSummary("Update transaction type")]
        [HttpPatch("update/{id}")]
        public async Task<Result> UpdateTransactionType(Guid id, int transactionType, CancellationToken cancellationToken)
            => await _updateTransactionService.UpdateTransactionType(id, transactionType, cancellationToken);

        [EndpointSummary("Force rollback transaction")]
        [HttpPatch("force-rollback/{id}")]
        public async Task<Result> ForceRollbackTransaction(Guid id, CancellationToken cancellationToken)
            => await _updateTransactionService.ForceRollbackTransaction(id, cancellationToken);
    }
}
