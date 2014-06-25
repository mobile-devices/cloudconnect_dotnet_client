using Couchbase;
using CouchbaseModelViews.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CloudConnect.CouchBaseProvider
{
    public static class CouchbaseManager
    {
        public static void RegisterModelViews(IEnumerable<Assembly> assemblies)
        {
            var builder = new ViewBuilder();
            builder.AddAssemblies(assemblies.ToList());
            var designDocs = builder.Build();
            var ddManager = new DesignDocManager();
            ddManager.Create(designDocs);
        }
    }

}
