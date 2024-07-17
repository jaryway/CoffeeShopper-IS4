using Jaryway.DynamicSpace.WebApi.Models;

namespace Jaryway.DynamicSpace.WebApi.Services
{
	public interface ICoffeeShopService
	{
		Task<List<CoffeeShopModel>> List();
	}
}
