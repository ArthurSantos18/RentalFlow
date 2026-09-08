using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace RentalFlow.API.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController(ICommandMediator _commandMediator, IQueryMediator _queryMediator) : ControllerBase
    {
    }
}
