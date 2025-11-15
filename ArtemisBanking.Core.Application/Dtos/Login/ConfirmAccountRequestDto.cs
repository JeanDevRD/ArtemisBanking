using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Login
{
    public class ConfirmAccountRequestDto
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }
    }
}
