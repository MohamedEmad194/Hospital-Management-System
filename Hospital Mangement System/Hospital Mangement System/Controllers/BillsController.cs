using Microsoft.AspNetCore.Mvc;
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
        /// Get all bills
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetBills()
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bills");
                return StatusCode(500, "An error occurred while retrieving bills");
            }
        }

        /// <summary>
        /// Get bill by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BillDto>> GetBill(int id)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(id);
                if (bill == null)
                    return NotFound($"Bill with ID {id} not found");

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
