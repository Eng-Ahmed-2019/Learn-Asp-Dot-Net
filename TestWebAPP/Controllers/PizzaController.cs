using TestWebAPP.Enums;
using TestWebAPP.Modles;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace TestWebAPP.Controllers
{
    //[Authorize]
    [ApiController]
    //[AllowAnonymous]
    [Route("[controller]")]
    //[Route("api/[controller]")]
    //[Authorize(Roles = nameof(Roles.Admin))]
    public class PizzaController : ControllerBase
    {
        private static List<Pizza> pizzas = new List<Pizza>()
        {
            new Pizza
            {
                Id = 1,
                Name = "Margherita",
                Price = 120,
                IsGlutenFree = false
            },
            new Pizza
            {
                Id = 2,
                Name = "Pepperoni",
                Price = 150,
                IsGlutenFree = false
            },
            new Pizza
            {
                Id = 3,
                Name = "Vegetarian",
                Price = 130,
                IsGlutenFree = true
            }
        };

        [AllowAnonymous]
        [HttpGet("Get-all-pizzas")]
        public ActionResult<List<Pizza>> GetAll()
        {
            return pizzas;
        }

        [Authorize]
        [HttpGet("Get-one-pizza/{id}")]
        public ActionResult<Pizza> GetPizza([FromRoute] int id)
        {
            var myPizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (myPizza == null) return NotFound("This pizza not found");

            return Ok(myPizza);
        }

        [Authorize]
        [HttpGet("Search-pizzaByName")]
        public ActionResult<List<Pizza>> SearchPizza([FromQuery] string name)
        {
            var result = pizzas
                .Where(p => p.Name.Contains(name ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("Filter-pizza")]
        public ActionResult<List<Pizza>> FilterPizza
        (
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice
        )
        {
            var result = pizzas
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("Check-client")]
        public ActionResult CheckClient
        (
            [FromHeader(Name = "X-Client-Name")] string clientName
        )
        {
            return Ok(new
            {
                Message = "Request received",
                Client = clientName
            });
        }

        //[Authorize(Roles = nameof(Roles.Admin))]
        //[Authorize(Roles = "Admin,Manager")]
        //[Authorize(Roles = nameof(Roles.Admin))]
        //[Authorize(Roles = nameof(Roles.Manager))]
        //[Authorize(Roles ="Admin")]
        //[Authorize(Policy = "over18only")] // program.cs // use instead of Roles ...
        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [HttpGet("Test-logger")]
        public ActionResult TestLogger
        (
            [FromServices] ILogger<PizzaController> logger
        )
        {
            logger.LogInformation("Welcome with you here");

            return Ok("Logger service is working");
        }

        [Authorize]
        [HttpGet("Get-user")]
        public IActionResult GetUser()
        {
            /*
            Claim? userid = user.FindFirst(
                ClaimTypes.NameIdentifier
            );
            string id = userid?.Value ?? "";

            Claim? email = user.FindFirst(
                ClaimTypes.Email
            );
            string em = email?.Value ?? "";
            */
            ClaimsPrincipal user = HttpContext.User;

            string? userId = user.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            string? email = user.FindFirst(
                ClaimTypes.Email
            )?.Value;

            bool r = user?.Identity?.IsAuthenticated ?? false;
            if (r != true) return BadRequest("not Authenticated");
            return Ok(new
            {
                Message = "Authenticated",
                data = new
                {
                    UserId = userId,
                    Email = email
                }
            });
        }

        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [HttpPost("Add-new-pizza")]
        public ActionResult<Pizza> AddPizza([FromBody] Pizza newPizza)
        {
            newPizza.Id = pizzas.Count + 1;

            pizzas.Add(newPizza);

            return CreatedAtAction
                (
                    nameof(GetPizza),
                    new { id = newPizza.Id },
                    newPizza
                );
        }

        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [ValidateAntiForgeryToken]
        [HttpPost("Add-Pizza-fromform")]
        public ActionResult<Pizza> AddPizzaFromForm
        (
            [FromForm] Pizza newPizza
        )
        {
            newPizza.Id = pizzas.Count + 1;

            pizzas.Add(newPizza);

            return CreatedAtAction
                (
                    nameof(GetPizza),
                    new { id = newPizza.Id },
                    newPizza
                );
        }

        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [HttpPut("Update-pizza/{id}")]
        public ActionResult<Pizza> UpdatePizza([FromRoute] int id, [FromBody] Pizza pizza)
        {
            var mypizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (mypizza == null) return NotFound("Not Found This Pizza here, Try again");

            mypizza.Name = pizza.Name;
            mypizza.Price = pizza.Price;
            mypizza.IsGlutenFree = pizza.IsGlutenFree;

            return Ok(mypizza);
        }

        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [HttpPatch("Update-pizza/{id}")]
        [Consumes("application/json-patch+json")]
        public ActionResult<Pizza> PatchPizza([FromRoute] int id, [FromBody] JsonPatchDocument<Pizza> json)
        {
            var myPizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (myPizza == null) return NotFound("This pizza not found, Try again!");
            if (json == null) return BadRequest("Try again!");

            bool patchError = false;
            json.ApplyTo(myPizza, error =>
            {
                patchError = true;

                ModelState.AddModelError(
                    "Patch",
                    error.ErrorMessage
                );
            });
            if (patchError || !ModelState.IsValid) return BadRequest(ModelState);

            return Ok(myPizza);
        }

        [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Manager)}")]
        [HttpDelete("Delete-pizza/{id}")]
        public ActionResult<string> DeletePizza([FromRoute] int id)
        {
            var mypizza = pizzas.FirstOrDefault(p => p.Id == id);
            if (mypizza == null) return NotFound("Not Found This Pizza here, Try again");

            pizzas.Remove(mypizza);

            return "Pizza Deleted Successfully";
        }
    }
}