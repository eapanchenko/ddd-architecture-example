using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DDD.Example.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : ControllerBase {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ExampleController(IMediator mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }
    }
}