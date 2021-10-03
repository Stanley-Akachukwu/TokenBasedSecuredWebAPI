using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class UserManagerResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Token { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
