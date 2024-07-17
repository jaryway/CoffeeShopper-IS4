using Jaryway.DynamicSpace.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jaryway.DynamicSpace.WebApi.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	[Authorize()]

	public class CoffeeShopController : ControllerBase
	{
        private readonly ICoffeeShopService coffeeShopService;

		public CoffeeShopController(ICoffeeShopService coffeeShopService)
		{
			this.coffeeShopService = coffeeShopService;
		}

		[HttpGet]
		public async Task<IActionResult> List()
		{
			var coffeeShops = await coffeeShopService.List();
			return Ok(coffeeShops);
		}
	}
}
