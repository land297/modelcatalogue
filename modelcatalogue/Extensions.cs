using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue
{
    static class Extensions
    {
        public static bool MemberWithAttributeValue(this DbElement element, DbAttribute attribute, string value) {
            DbElement y = element.Members().SingleOrDefault(x => x.GetAsString(attribute) == value);
            if (y != null && y.IsValid) {
                return true;
            }
            return false;
        }
        public static int CountOfMembes(this DbElement element) {
            DbElement[] members = element.Members();
            return members.Count();
            
        }

        public static string Clean(this string s) {
            s = s.Replace("(","");
            s = s.Replace(")", "");
            return s.Trim();
        }

        public static string Purpose(this DbElement element) {
            var value = string.Empty;
            element.GetValidAsString(DbAttributeInstance.PURP, ref value);
            return value;
        }

        public static int Count(this DBElementCollection collection) {
            int i = 0;
            foreach (var item in collection) {
                i++;
            }
            return i;
        }

        public static DbElement First(this DBElementCollection collection) {
            foreach (DbElement item in collection) {
                return item;
            }
            return default(DbElement);
        }
    }
}
