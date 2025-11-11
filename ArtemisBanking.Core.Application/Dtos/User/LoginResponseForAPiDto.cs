using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.User
{
    public class LoginResponseForAPiDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? AccessToken { get; set; }
        public bool HasError { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
