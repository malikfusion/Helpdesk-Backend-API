using Helpdesk_Backend_API.Entities.NonDbEntities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Helpdesk_Backend_API.Utilities
{
    public static class RegexFind
    {
        public static Task<bool> FindNameByRegex<T>(this IQueryable<T> entity, string text) where T : BaseEntity
        {
            return entity
                .AnyAsync(c => Regex.IsMatch(c.Name, text, RegexOptions.IgnoreCase));
        }

        public static IQueryable<T> FilterNameByRegex<T>(this IQueryable<T> entity, string text) where T : BaseEntity
        {
            return entity
                .Where(c => Regex.IsMatch(c.Name, text, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }
    }
}
