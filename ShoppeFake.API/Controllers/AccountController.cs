using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [SwaggerOperation(summary: "Create a new account", description: "Creates a new user account with the provided details.")]
        public async Task<IActionResult> CreateAccount(AccountRequest request)
        {
            var result = await _accountService.CreateAccount(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<AccountResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<AccountResponse>.OkResponse(result.Value, "Account created successfully.", "201"));
        }

        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Get account by ID", description: "Retrieves the details of an account using its unique identifier.")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var result = await _accountService.GetAccountById(id);
            if (result.IsFailure)
            {
                return NotFound(ApiResponse<AccountResponse>.NotFoundResponse(result.Error.Message));
            }
            return Ok(ApiResponse<AccountResponse>.OkResponse(result.Value, "Account retrieved successfully.", "200"));
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get all accounts", description: "Retrieves a paginated list of all accounts.")]
        public async Task<IActionResult> GetAllAccounts(int pageIndex = 1, int pageSize = 10)
        {

            var result = await _accountService.GetAllAccounts(pageIndex, pageSize);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<BasePaginatedList<AccountResponse>>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<AccountResponse>>.OkResponse(result.Value, "Accounts retrieved successfully.", "200"));
        }

        [HttpPut]
        [SwaggerOperation(summary: "Update an account", description: "Updates the details of an existing account.")]
        public async Task<IActionResult> UpdateAccount(AccountRequest request)
        {
            var result = await _accountService.UpdateAccount(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<AccountResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<AccountResponse>.OkResponse(result.Value, "Account updated successfully.", "200"));
        }

        [HttpDelete]
        [SwaggerOperation(summary: "Delete an account", description: "Deletes an account using its unique identifier.")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAccount(id);
            return NoContent();
        }

    }
}
