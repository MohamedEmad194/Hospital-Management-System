using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly ILogger<BillsController> _logger;

        public BillsController(IBillService billService, ILogger<BillsController> logger)
        {
            _billService = billService;
            _logger = logger;
        }

        /// <summary>
        /// Get all bills (filtered by role)
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills()
        {
            try
            {
                // Check if user is authenticated - [Authorize] should handle this, but double-check
                if (User?.Identity == null || !User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthenticated request to GetBills - User.Identity is null or not authenticated");
                    return StatusCode(401, new { message = "Authentication required", details = "Please login to access this resource" });
                }

                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                _logger.LogInformation("GetBills called - Role: {Role}, PatientId: {PatientId}", 
                    userRole, patientIdClaim);

                IEnumerable<BillDto> bills;

                if (userRole == "Admin" || userRole == "Doctor" || userRole == "Staff")
                {
                    // Admin, Doctor, Staff see all bills
                    bills = await _billService.GetAllBillsAsync();
                }
                else if (userRole == "Patient" && !string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId))
                {
                    // Patient sees only their bills
                    bills = await _billService.GetBillsByPatientAsync(patientId);
                }
                else
                {
                    _logger.LogWarning("Insufficient permissions - Role: {Role}, PatientId: {PatientId}", userRole, patientIdClaim);
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view bills" });
                }

                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return StatusCode(500, new 
                { 
                    message = "An error occurred while retrieving bills",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    details = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Get bill by ID (filtered by role)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BillDto>> GetBill(int id)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(id);
                if (bill == null)
                    return NotFound($"Bill with ID {id} not found");

                // Check permissions
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var patientIdClaim = User.FindFirstValue("PatientId");

                if (userRole == "Patient" && (!string.IsNullOrEmpty(patientIdClaim) && int.TryParse(patientIdClaim, out int patientId)))
                {
                    if (bill.PatientId != patientId)
                        return StatusCode(403, new { message = "Insufficient permissions", details = "You can only view your own bills" });
                }
                else if (userRole != "Admin" && userRole != "Doctor" && userRole != "Staff")
                {
                    return StatusCode(403, new { message = "Insufficient permissions", details = "You do not have access to view this bill" });
                }

                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bill with ID {BillId}", id);
                return StatusCode(500, "An error occurred while retrieving the bill");
            }
        }

        /// <summary>
        /// Get bills by patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBillsByPatient(int patientId)
        {
            try
            {
                var bills = await _billService.GetBillsByPatientAsync(patientId);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while retrieving bills");
            }
        }

        /// <summary>
        /// Get overdue bills
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetOverdueBills()
        {
            try
            {
                var bills = await _billService.GetOverdueBillsAsync();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue bills");
                return StatusCode(500, "An error occurred while retrieving overdue bills");
            }
        }

        /// <summary>
        /// Get total outstanding amount
        /// </summary>
        [HttpGet("outstanding-amount")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<decimal>> GetTotalOutstandingAmount()
        {
            try
            {
                var amount = await _billService.GetTotalOutstandingAmountAsync();
                return Ok(new { totalOutstandingAmount = amount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total outstanding amount");
                return StatusCode(500, "An error occurred while retrieving total outstanding amount");
            }
        }

        /// <summary>
        /// Create a new bill
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<BillDto>> CreateBill(CreateBillDto createBillDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var bill = await _billService.CreateBillAsync(createBillDto);
                return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                return StatusCode(500, "An error occurred while creating the bill");
            }
        }

        /// <summary>
        /// Update an existing bill
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<BillDto>> UpdateBill(int id, UpdateBillDto updateBillDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var bill = await _billService.UpdateBillAsync(id, updateBillDto);
                if (bill == null)
                    return NotFound($"Bill with ID {id} not found");

                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill with ID {BillId}", id);
                return StatusCode(500, "An error occurred while updating the bill");
            }
        }

        /// <summary>
        /// Process payment for a bill
        /// </summary>
        [HttpPost("{id}/payment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> ProcessPayment(int id, PaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _billService.ProcessPaymentAsync(id, paymentDto);
                if (!result)
                    return NotFound($"Bill with ID {id} not found");

                return Ok(new { message = "Payment processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for bill {BillId}", id);
                return StatusCode(500, "An error occurred while processing payment");
            }
        }

        /// <summary>
        /// Delete a bill
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBill(int id)
        {
            try
            {
                var result = await _billService.DeleteBillAsync(id);
                if (!result)
                    return NotFound($"Bill with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill with ID {BillId}", id);
                return StatusCode(500, "An error occurred while deleting the bill");
            }
        }

        /// <summary>
        /// Check if bill exists
        /// </summary>
        [HttpHead("{id}")]
        [Authorize]
        public async Task<ActionResult> BillExists(int id)
        {
            try
            {
                var exists = await _billService.BillExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if bill exists with ID {BillId}", id);
                return StatusCode(500, "An error occurred while checking bill existence");
            }
        }
    }
}
