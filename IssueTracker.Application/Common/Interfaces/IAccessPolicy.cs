using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.Interfaces
{
    public interface IAccessPolicy<T>
    {
        T? Apply(T accessedObject, string userId);
    }
}
