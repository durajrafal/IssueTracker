using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base()
        {
        }

        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public NotFoundException(string name, string id)
            : base($"Entity {name} with id={id} not found.")
        {
        }        
        
        public NotFoundException(string name, string id, Exception? innerException)
            : base($"Entity {name} with id={id} not found.", innerException)
        {
        }
    }
}
