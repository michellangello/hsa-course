using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace QueueService.Controllers;

[ApiController]
[Route("redis-rdb")]
public class RedisRdbController([FromKeyedServices("RedisRdb")]IConnectionMultiplexer redisMultiplexer) : ControllerBase
{
	private readonly IDatabase _redis = redisMultiplexer.GetDatabase();
	private readonly Random _random = new();

	[HttpPost("push")]
	public async Task<IActionResult> PushMessage()
	{
		var message = $"RDB_Message_{_random.Next(1000, 9999)}";
		var d = _redis.ListLeftPush("queue:rdb", message);
		return Ok();
	}

	[HttpGet("pull")]
	public async Task<IActionResult> PullMessage()
	{
		var message = await _redis.ListRightPopAsync("queue:rdb");

		return message.HasValue
			? Ok(new { Message = message.ToString() })
			: NoContent();
	}
}
