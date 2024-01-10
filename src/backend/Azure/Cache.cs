using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FunctionApp.Azure
{
    public interface ICache
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<(T Value, TimeSpan Expiration)>> valueConstructor)
            where T : class;
    }

    public class Cache : ICache
    {
        private readonly IDatabase db;

        public Cache(string connectionString)
        {
            var connector = ConnectionMultiplexer.Connect(connectionString);
            db = connector.GetDatabase();
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<(T Value, TimeSpan Expiration)>> valueConstructor)
            where T : class
        {
            var redisValue = await db.StringGetAsync(key);

            if (redisValue == RedisValue.Null) 
            {
                var data = await valueConstructor();

                var str = JsonConvert.SerializeObject(data.Value);

                await db.StringSetAsync(key, str);
                await db.KeyExpireAsync(key, DateTime.Now.Add(data.Expiration));

                return data.Value;
            }
            else
            {
                var result = JsonConvert.DeserializeObject<T>(redisValue.ToString())
                             ?? throw new Exception($"Unable to deseriaize cached value for key: {key}");

                return result;
            }            
        }
    }
}
