using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using InMemoryCachingWebApi.Context;
using InMemoryCachingWebApi.Dtos;
using InMemoryCachingWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace InMemoryCachingWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController:ControllerBase
    {
        private readonly StudentContext _studentContext;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributedCache;

        public StudentController(IMemoryCache cache,StudentContext studentContext,IDistributedCache distributedCache)
        {
            _studentContext=studentContext;
            _cache=cache;
            _distributedCache=distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // var students= await _studentContext.Students.ToListAsync();
            // return Ok(students);

            var studentsKey="students";

            if (!_cache.TryGetValue(studentsKey, out List<Student> students))
            {
                students= await _studentContext.Students.ToListAsync();

                var cacheExpiryOptions= new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration=DateTime.Now.AddMinutes(5),
                    Priority=CacheItemPriority.High,
                    SlidingExpiration=TimeSpan.FromMinutes(2),
                    Size=1024,

                };
                _cache.Set(studentsKey,students,cacheExpiryOptions);

            }
            return Ok(students);
        }

        [HttpGet("redis")]
        public async  Task<IActionResult> GetAllStudentsFromRedis()
        {
            string cacheKey="studentsList";
            string serializedStudentsList;
            var students=new List<Student>();

            var redisCustomerList= await _distributedCache.GetAsync(cacheKey);
            if (redisCustomerList!=null)
            {
                serializedStudentsList=Encoding.UTF8.GetString(redisCustomerList);
                students= JsonConvert.DeserializeObject<List<Student>>(serializedStudentsList);
            }
            else
            {
                students= await _studentContext.Students.ToListAsync();
                serializedStudentsList=JsonConvert.SerializeObject(students);
                redisCustomerList=Encoding.UTF8.GetBytes(serializedStudentsList);

                var cacheExpiryOptions= new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                await _distributedCache.SetAsync(cacheKey,redisCustomerList,cacheExpiryOptions);
            }

            return Ok(students);
        }
 
        [HttpPost]
        public async Task<IActionResult> PostStudent()
        {

            var studentList=new List<Student>();
            for (int i = 0; i <= 1000; i++)
            {
                var studentDto= new Student()
                {
                    Address="Address "+i,
                    Name="Dinesh "+i,
                    Age=1,
                    School="Boisar "+i,
                    Email="test@gmail.com"
                    
                };

                studentList.Add(studentDto);
            }

            await _studentContext.Students.AddRangeAsync(studentList);

            var result= await _studentContext.SaveChangesAsync();
            if (result>0)
            {
                return Ok("Students Created Successfully");
            }

            return BadRequest("Student Not Created");
        }
        
    }
}