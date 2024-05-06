using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCachingWebApi.Models
{
    public class CacheRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}