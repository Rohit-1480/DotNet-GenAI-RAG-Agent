using GenAI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAI.Infrastructure.Context
{
    public class SimpleContextProvider : IContextProvider
    {
        public Task<string> GetContextAsync(string query)
        {
            //throw new NotImplementedException();
            var context = """
        This application is built using ASP.NET Core and follows
        dependency injection and clean architecture principles.
        The application uses interfaces to keep components loosely coupled.
        """;

            return Task.FromResult(context);
        }
    }
}
