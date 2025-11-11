using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.User
{
    public class JwtResponseDto
    {
        public bool HasError { get; set; }
        public required string Error { get; set; }
    }
}
