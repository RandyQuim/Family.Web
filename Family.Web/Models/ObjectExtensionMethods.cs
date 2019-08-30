using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Family.Web.Models
{
    public static class ObjectExtensionMethods
    {
        /// <summary>
        /// Makes a specified object into a queryable
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="instance">The instance of the object</param>
        /// <returns>The object as queryable</returns>
        public static IQueryable<T> ToQueryable<T>(this T instance)
        {
            return new[] { instance }.AsQueryable();
        }
    }
}