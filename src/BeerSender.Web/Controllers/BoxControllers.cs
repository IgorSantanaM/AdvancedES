using BeerSender.Domain;
using BeerSender.Domain.Boxes.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BeerSender.Web.Controllers;

[ApiController]
[Route("api/command/[controller]")]
public class BoxControllers(CommandRouter router) : ControllerBase
{
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateBox([FromBody] CreateBox command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }

    [HttpPost]
    [Route("add-bottle")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateBox([FromBody] AddBeerBottle command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }

    [HttpPost]
    [Route("add-label")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateBox([FromBody] AddShippingLabel command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }

    [HttpPost]
    [Route("close")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateBox([FromBody] CloseBox command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }

    [HttpPost]
    [Route("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateBox([FromBody] SendBox command)
    {
        await router.HandleCommand(command);
        return Accepted();
    }
}