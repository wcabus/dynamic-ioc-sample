using System.Collections.Generic;
using Sample.Contracts;

namespace Sample.Repositories
{
    public class Repository : IRepository
    {
        public IEnumerable<string> GetData()
        {
            return new[] { "data1", "data2" };
        }
    }
}
