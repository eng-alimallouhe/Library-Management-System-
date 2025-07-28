using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.Identity;
using LMS.Application.Features.Identity.Notifications.Commands.ReadNotification;
using LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotifications;
using LMS.Application.Features.Identity.Notifications.Queries.GetUnreadNotificationsCount;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.identity
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/identity/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;


        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;   
        }


        [HttpGet("unread")]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetUnreadNotification([FromQuery]Guid userId, [FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            return Ok((await _mediator.Send(new GetUnreadNotificationsQuery(userId, pageNumber, pageSize))));
        }


        [HttpGet("unread/count")]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetUnreadNotificationCountAsync([FromQuery] Guid userId)
        {
            return Ok((await _mediator.Send(new GetUnreadNotificationsCountQuery(userId))));
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> ReadNotification(Guid id)
        {
            var response = await _mediator.Send(new ReadNotificationCommand()
            {
                NotificationId = id
            });

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
