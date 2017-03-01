using System.Collections.Generic;

namespace Sample.Contracts
{
    public interface IRepository
    {
        IEnumerable<string> GetData();
    }
}
