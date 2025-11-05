using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Domain.Common
{
    public class CommonEntity<TKey>
    {
        public required TKey Id { get; set; }
    }
}
