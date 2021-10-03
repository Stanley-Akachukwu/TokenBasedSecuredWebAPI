using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.WebAPI.Data
{
    public class UCareDbContext:DbContext
    {
        public UCareDbContext(DbContextOptions<UCareDbContext> options)
       : base(options) { }
    }
}
