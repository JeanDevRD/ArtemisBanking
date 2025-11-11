using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.User
{
    public class ForgotPasswordRequestDto
    {
        public required string UserName { get; set; }
        public string? Origin { get; set; }
    }
}
