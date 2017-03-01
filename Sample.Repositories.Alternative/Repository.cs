using System.Collections.Generic;
using Sample.Contracts;

namespace Sample.Repositories.Alternative
{
    public class Repository : IRepository
    {
        public IEnumerable<string> GetData()
        {
            return new[] { "data3", "data4" };
        }
    }
}
