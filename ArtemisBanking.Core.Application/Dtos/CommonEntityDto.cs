using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos
{
    public class CommonEntityDto<Tkey>
    {
        public required Tkey Id { get; set; }
    }
}
