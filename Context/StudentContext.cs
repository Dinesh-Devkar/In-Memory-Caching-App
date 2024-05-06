using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InMemoryCachingWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCachingWebApi.Context
{
    public class StudentContext:DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
    }
}