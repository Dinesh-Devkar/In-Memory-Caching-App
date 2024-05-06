using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InMemoryCachingWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InMemoryCachingWebApi.Controllers
{   [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        public CacheController(IMemoryCache memoryCache)
        {
            _memoryCache=memoryCache;
        }

        [HttpGet("{key}")]
        public IActionResult GetCache(string key)
        {
            string value=string.Empty;

            _memoryCache.TryGetValue(key, out value);
            return Ok(value);
        }

        [HttpPost]
        public IActionResult SetCache(CacheRequest request)
        {
            var cacheExpiryOptions=new MemoryCacheEntryOptions
            {
                AbsoluteExpiration=DateTime.Now.AddMinutes(5),
                Priority=CacheItemPriority.High,
                SlidingExpiration=TimeSpan.FromMinutes(2),
                Size=1024

            };
            _memoryCache.Set(request.Key,request.Value,cacheExpiryOptions);
            return Ok("Cache Set Successfully");
        }
    }
}