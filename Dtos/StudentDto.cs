using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCachingWebApi.Dtos
{
    public class StudentDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string School { get; set; }
        public string Email { get; set; }
    }
}