using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlashSale.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashSaleAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FlashSaleAdminController> _logger;

        public FlashSaleAdminController(IMediator mediator, ILogger<FlashSaleAdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost("admin/sync")]
        public async Task<IActionResult> SyncAllActiveItems([FromQuery] bool forceSync = false)
        {
            _logger.LogInformation("Manual sync all active items requested, ForceSync: {ForceSync}", forceSync);

            try
            {
                var command = new SyncAllActiveItemsCommand("Manual API Request", forceSync);
                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("Sync all completed successfully: {ItemsProcessed} items, {Duration}ms",
                        result.ItemsProcessed, result.DurationMs);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Sync all completed with errors: {SuccessfulSyncs} success, {FailedSyncs} failed",
                        result.SuccessfulSyncs, result.FailedSyncs);
                    return StatusCode(500, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sync all active items command");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error occurred",
                    Error = ex.Message
                });
            }
        }
        [HttpPost("admin/sync/{itemId:guid}")]
        public async Task<IActionResult> SyncSpecificItem(Guid itemId, [FromQuery] bool forceSync = false)
        {
            _logger.LogInformation("Manual sync specific item requested for {ItemId}, ForceSync: {ForceSync}",
                itemId, forceSync);

            try
            {
                var command = new SyncSpecificItemCommand(itemId, "Manual API Request", forceSync);
                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("Sync specific item completed successfully for {ItemId}, Duration: {Duration}ms",
                        itemId, result.DurationMs);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Sync specific item failed for {ItemId}: {Message}",
                        itemId, result.Message);

                    // Return appropriate status code based on error type
                    if (result.Error?.Contains("not found") == true)
                    {
                        return NotFound(result);
                    }
                    else
                    {
                        return StatusCode(500, result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sync specific item command for {ItemId}", itemId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error occurred",
                    Error = ex.Message,
                    FlashSaleItemId = itemId
                });
            }
        }
    }
}

