using Application.ProductFeatures.Commands;
using Application.ProductFeatures.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MovieManager.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MovieController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllMoviesQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await Mediator.Send(new GetMovieByIdQuery { Id = id });

                return Ok(result);
            }
            catch (Exception ex) 
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await Mediator.Send(new DeleteMovieByIdCommand { Id = id });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update(int id, UpdateMovieCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    return BadRequest();
                }

                var result = await Mediator.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
