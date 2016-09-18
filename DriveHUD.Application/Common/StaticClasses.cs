using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Model.Enums;

using DriveHUD.Application.TableConfigurators;

namespace DriveHUD.Application.Common.Static
{   
    public static class Helpers
    {
        static public string FindNameFromResource(object resourceItem)
        {
            foreach (ResourceDictionary dictionary in App.Current.Resources.MergedDictionaries)
            {
                foreach (object key in dictionary.Keys)
                {
                    if (dictionary[key] == resourceItem)
                    {
                        return key.ToString();
                    }
                }
            }
            return null;
        }
    }
}
