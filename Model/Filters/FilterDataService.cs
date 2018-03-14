//-----------------------------------------------------------------------
// <copyright file="FilterDataService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Model.Filters
{
    internal class FilterDataService : IFilterDataService
    {
        private static readonly string _defaultFilterFileName = "filter.df";
        private static object lockObject = new object();

        private string _defaultFolderPath;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="defaultFolder">Folder to store default filter in</param>
        public FilterDataService(string defaultFolderPath)
        {
            _defaultFolderPath = defaultFolderPath;
        }

        public void SaveDefaultFilter(Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> _filtersDictionary)
        {
            foreach (var filter in _filtersDictionary)
            {
                SaveFilter(filter.Value, Path.Combine(_defaultFolderPath, GetFilterFileName(filter.Key)));
            }
        }

        public void SaveFilter(ICollection<IFilterModel> filtersCollection, string path)
        {
            try
            {
                lock (lockObject)
                {
                    using (var fs = File.CreateText(path))
                    {
                        var serializer = new JsonSerializer()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                        };

                        serializer.Serialize(fs, filtersCollection);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not save filter at {path}", e);
            }
        }

        public void LoadDefaultFilter(Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> _filtersDictionary)
        {
            for (int i = 0; i < _filtersDictionary.Count(); i++)
            {
                var key = _filtersDictionary.ElementAt(i).Key;

                var loadedFiltersCollection = LoadFilter(Path.Combine(_defaultFolderPath, GetFilterFileName(key)));

                if (loadedFiltersCollection != null)
                {
                    foreach (var filter in _filtersDictionary[key])
                    {
                        var loadedFilter = loadedFiltersCollection.FirstOrDefault(x => x.GetType() == filter.GetType());
                        filter.LoadFilter(loadedFilter);
                    }
                }
            }
        }

        public ICollection<IFilterModel> LoadFilter(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                lock (lockObject)
                {
                    using (StreamReader fs = File.OpenText(path))
                    {
                        using (var jsonTextReader = new JsonTextReader(fs))
                        {
                            var serializer = new JsonSerializer()
                            {
                                TypeNameHandling = TypeNameHandling.Objects,
                                ObjectCreationHandling = ObjectCreationHandling.Replace
                            };

                            return serializer.Deserialize<Collection<IFilterModel>>(jsonTextReader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not load filter at {path}", e);
            }

            return null;
        }

        private string GetFilterFileName(EnumFilterType filterType)
        {
            return String.Format("{0}_{1}", filterType, _defaultFilterFileName);
        }
    }
}