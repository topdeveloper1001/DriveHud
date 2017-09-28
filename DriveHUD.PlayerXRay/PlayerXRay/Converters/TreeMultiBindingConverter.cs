using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using AcePokerSolutions.DataTypes.NotesTreeObjects;

namespace AcePokerSolutions.PlayerXRay.Converters
{
    public class TreeMultiBindingConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> result = new List<object>();

            foreach (Object obj in values)
            {
                if (obj is List<InnerGroupObject>)
                {
                    foreach (InnerGroupObject group in (List<InnerGroupObject>) obj)
                    {
                        result.Add(group);
                    }
                }
                if (obj is List<NoteObject>)
                {
                    foreach (NoteObject note in (List<NoteObject>)obj)
                    {
                        result.Add(note);
                    }
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
