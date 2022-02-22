
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using GreetingService.API.Authentication;
using GreetingService.Core.Interfaces;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GreetingService.API.Controllers
{

    [Route("api/[controller]")]
    [BasicAuth]  // You can optionally provide a specific realm --> [BasicAuth("my-realm")]
    [ApiController]
    public class GreetingController : ControllerBase
    {


        public readonly IGreetingRepository _greetingRepository;
        

        public GreetingController(IGreetingRepository greetingRepository)
        {
            _greetingRepository = greetingRepository;
        }


        




        // GET: api/<GreetingController>
        [HttpGet]
        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            //_greetingRepository.Create(new Greeting ());
            return await _greetingRepository.ReadAsync();
        }


        //GET api/<GreetingController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Greeting))]        //when we return IActionResult instead of Greeting, there is no way for swagger to know what the return type is, we need to explicitly state what it will return
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type =typeof(Greeting))]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var greeting = await _greetingRepository.GetAsync(id);
            if (greeting == null)
                return NotFound();

            return Ok(greeting);
        }

        // GET api/<GreetingController>/5
        //[HttpGet("{id}")]
        //public Greeting Get(Guid id)
        //{
        //    if(id == Guid.Empty)
        //    {
        //        return null;
        //    }

        //    return _greetingRepository.Get(id);
        //}


        // POST api/<GreetingController>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Greeting greeting)
        {
            try
            {
                await _greetingRepository.CreateAsync(greeting);
                return Accepted();
            }
            catch                       //any exception will result in 409 Conflict which might not be true but we'll use this for now
            {
                return Conflict();
            }
        }

        // POST api/<GreetingController>
        //[HttpPost]
        //public string Post([FromBody] Greeting greeting1)
        //{

        //    _greetingRepository.Create(greeting1);
        //    return "New API is added";


        //}



        // PUT api/<GreetingController>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put([FromBody] Greeting greeting)
        {
            try
            {
                _greetingRepository.UpdateAsync(greeting);
                return Accepted();
            }
            catch
            {
                return NotFound($"Greeting with {greeting.Id} not found");
            }
        }


        // PUT api/<GreetingController>/5
        //[HttpPut]
        //public string Put([FromBody] Greeting greeting)
        //{
        //    _greetingRepository.Update(greeting);
        //    return "API is now updated.";
            
        //}

        // DELETE api/<GreetingController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                Console.WriteLine("No such Id found...");
                return NotFound();
            }
            

            _greetingRepository.DeleteAsync(id);
            Console.WriteLine("API is removed.");
            return Accepted();
        }
    }
}
