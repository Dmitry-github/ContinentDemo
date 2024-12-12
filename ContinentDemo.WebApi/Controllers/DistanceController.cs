
namespace ContinentDemo.WebApi.Controllers
{
    using FluentValidation;
    using Interfaces;
    using Queries;
    using Responses;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;

    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly ILogger<DistanceController> _logger;
        private readonly IDistanceService _distanceService;

        public DistanceController(ILogger<DistanceController> logger, IDistanceService distanceService)
        {
            _logger = logger;
            _distanceService = distanceService;
        }

        [HttpPost]
        public async Task<IActionResult> GetDistanceAsync(DistanceQuery request, [FromServices] IValidator<DistanceQuery> validator)
        {
            var requestValidationResult = await RequestValidationAsync(request, validator);
            if (requestValidationResult != null)
                return requestValidationResult;

            var (distance, problemText) = await _distanceService.GetDistanceBetweenIataAsync(request.Iata1, request.Iata2);

            if (distance > 0)
                return Ok(new DistanceResponse { Iata1 = request.Iata1, Iata2 = request.Iata2, Distance = distance });

            return BadRequest(CreateProblemDetails("Logic error", $"Invalid result: {distance} - {problemText}"));
        }

        private async Task<IActionResult?> RequestValidationAsync(DistanceQuery request, IValidator<DistanceQuery> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(CreateProblemDetails("Validation error(s)", errorMessages));
            }

            return null;
        }

        //RFC 9457
        private ProblemDetails CreateProblemDetails(string type, string detail, int status = (int)HttpStatusCode.BadRequest)
        {
            return new ProblemDetails()
            {
                Type = type,
                Title = "Error",
                Status = status,
                Detail = $"{type}: {detail}",
                Instance = Request.Path
            };
        }
    }
}
