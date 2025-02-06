using System.Text;
using Beanstalk.Core;
using Microsoft.AspNetCore.Mvc;

namespace QueueService.Controllers;

[ApiController]
[Route("beanstalkd")]
public class BeanstalkdController(BeanstalkConnection beanstalk) : ControllerBase
{
	private readonly Random _random = new();

	[HttpPost("push")]
	public IActionResult PushMessage()
	{
		var message = $"Beanstalk_Message_{_random.Next(1000, 9999)}";

		beanstalk.Put(message);
		return Ok();
	}

	[HttpGet("pull")]
	public async Task<IActionResult> PullMessage()
	{
		var job = await beanstalk.Reserve(TimeSpan.FromSeconds(1));
		if (job != null)
		{
			var id = job.Id;
			await beanstalk.Delete(id);
		}

		return Ok();
	}
}
