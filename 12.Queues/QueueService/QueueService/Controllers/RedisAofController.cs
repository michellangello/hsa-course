using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace QueueService.Controllers;

[ApiController]
[Route("redis-aof")]
public class RedisAofController([FromKeyedServices("RedisAof")] IConnectionMultiplexer redisMultiplexer)
	: ControllerBase
{
	private readonly IDatabase _redis = redisMultiplexer.GetDatabase();
	private readonly Random _random = new();

	[HttpPost("push")]
	public async Task<IActionResult> PushMessage()
	{
		var message = $"AOF_Message_{_random.Next(1000, 9999)}";
		await _redis.ListLeftPushAsync("queue:aof", message);
		return Ok();
	}

	[HttpGet("pull")]
	public async Task<IActionResult> PullMessage()
	{
		var message = await _redis.ListRightPopAsync("queue:aof");

		return message.HasValue
			? Ok(new { Message = message.ToString() })
			: NoContent();
	}
}
