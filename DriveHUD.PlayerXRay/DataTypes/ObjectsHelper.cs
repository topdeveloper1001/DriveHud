#region Usings

using System;
using System.Collections;
using System.Reflection;

#endregion

namespace AcePokerSolutions.DataTypes
{
    public static class ObjectsHelper
    {
        public static int GetNextID(IList list)
        {
            int id = 1;

            if (list == null)
                return 1;

            Type listType = list.GetType();
            Type elemTypes = listType.GetGenericArguments()[0];

            MemberInfo[] members = elemTypes.GetMember("ID");

            if (members.Length <= 0)
                return -1;

            PropertyInfo info = (PropertyInfo) members[0];

            while (IDExists(id, list, info))
                id++;

            return id;
        }

        private static bool IDExists(int id, IList list, PropertyInfo info)
        {
            foreach (var obj in list)
            {
                if ((int) info.GetValue(obj, null) == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}