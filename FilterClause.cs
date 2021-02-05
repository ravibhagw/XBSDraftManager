using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XBS.Core;

namespace DraftUtility
{
    public static class FilterClause
    {
        public static bool GetByPosition(Player player, bool? showPosition, PropertyInfo property)
        {
            

            int? minAvailability = showPosition == true ? 0 : 4;
            int? pos = (int?)property.GetValue(player);

            return pos > minAvailability && pos.HasValue == showPosition;
        }

        public static bool GetBySearchString(Player player, string value)
        {
            if(String.IsNullOrEmpty(value))
            {
                return true;
            }
            return new CultureInfo(1033).CompareInfo.IndexOf(player.Gamertag, value, CompareOptions.OrdinalIgnoreCase) >= 0; 
        }
    }
}
