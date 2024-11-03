using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Models.ReservationRequest;
using Microsoft.AspNetCore.Mvc;

namespace IISBackend.API.Controllers
{
    [ApiController]
    [Route("request")]
    public class ReservationRequestController : ControllerBase
    {
        private readonly IReservationRequestFacade _facade;

        public ReservationRequestController(IReservationRequestFacade facade)
        {
            _facade = facade;
        }

        [HttpPost]
        public async Task<ActionResult<ReservationRequestDetailModel>> Create(ReservationRequestCreateModel model)
        {
            var result = await _facade.SaveAsync(model);
            if (result != null)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            return BadRequest("Unable to create reservation request.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationRequestDetailModel>> GetById(Guid id)
        {
            var result = await _facade.GetAsync(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationRequestListModel>>> GetAll()
        {
            var results = await _facade.GetAsync();
            return Ok(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _facade.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{requestId}/approve")]
        public async Task<IActionResult> Approve(Guid requestId)
        {
            await _facade.ApproveRequestAsync(requestId);
            return Ok();
        }

        [HttpPut("{requestId}/reject")]
        public async Task<IActionResult> Reject(Guid requestId)
        {
            await _facade.RejectRequestAsync(requestId);
            return Ok();
        }
    }
}