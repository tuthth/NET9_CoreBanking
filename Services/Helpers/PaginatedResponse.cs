using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class PaginatedResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public string? NextCursor { get; set; }

        public PaginatedResponse(IEnumerable<T> data, string? nextCursor)
        {
            Data = data;
            NextCursor = nextCursor;
        }
    }
}
