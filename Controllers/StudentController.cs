using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InMemoryCachingWebApi.Context;
using InMemoryCachingWebApi.Dtos;
using InMemoryCachingWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCachingWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController:ControllerBase
    {
        private readonly StudentContext _studentContext;
        private readonly IMemoryCache _cache;

        public StudentController(IMemoryCache cache,StudentContext studentContext)
        {
            _studentContext=studentContext;
            _cache=cache;
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