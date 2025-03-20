using System;
using System.Reflection;
using System.Text;
using Entitites.Models;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var OrderByQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(' ')[0];
                var objectProperty = propertyInfo.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty is null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                OrderByQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
            }

            var orderQuery = orderByQueryString.ToString().TrimEnd(',', ' ');
            return orderQuery;
        }
    }
}
