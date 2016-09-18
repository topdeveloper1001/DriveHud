using DriveHUD.Common.Log;
using Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Filters
{
    public class FilterDataService : IFilterDataService
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

        public void SaveDefaultFilter(Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> _filtersDictionary)
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
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.Objects,
                            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                        };
                        serializer.Serialize(fs, filtersCollection);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        public void LoadDefaultFilter(Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> _filtersDictionary)
        {
            for (int i = 0; i < _filtersDictionary.Count(); i++)
            {
                var key = _filtersDictionary.ElementAt(i).Key;

                var loadedFiltersCollection = LoadFilter(Path.Combine(_defaultFolderPath, GetFilterFileName(key)));
                if (loadedFiltersCollection != null)
                {
                    //_filtersDictionary[key] = new ObservableCollection<IFilterModel>(loadedFiltersCollection);
                    foreach(var filter in _filtersDictionary[key])
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
                    using (JsonTextReader jsonTextReader = new JsonTextReader(fs))
                    {
                        JsonSerializer serializer = new JsonSerializer()
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            ObjectCreationHandling = ObjectCreationHandling.Replace
                        };

                        return serializer.Deserialize<Collection<IFilterModel>>(jsonTextReader);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            return null;
        }

        private string GetFilterFileName(EnumFilterType filterType)
        {
            return String.Format("{0}_{1}", filterType, _defaultFilterFileName);
        }
    }
}
