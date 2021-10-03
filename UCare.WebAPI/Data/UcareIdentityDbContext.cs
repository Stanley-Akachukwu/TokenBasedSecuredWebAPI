using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.WebAPI.Data
{
    public class UcareIdentityDbContext: IdentityDbContext
    {
        public UcareIdentityDbContext(DbContextOptions<UcareIdentityDbContext> options)
      : base(options) { }
    }
}
