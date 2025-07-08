using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Ascon.Pilot.SDK;

namespace MyIceLibrary
{
    public static class DataGridHelper
    {
        public static IEnumerable<AttributeValues> BringTogetherAllAttributes(IDictionary<string, object>[] inputData)
        {
            try
            {
                var allAttributes = inputData
                    .SelectMany(dict => dict.Keys)
                    .Distinct()
                    .OrderBy(key => key)
                    .ToList();

                var result = new List<AttributeValues>();

                foreach (var attr in allAttributes)
                {
                    var row = new AttributeValues
                    {
                        AttributeName = attr,
                        Values = new List<object>()
                    };

                    foreach (var dict in inputData)
                    {
                        row.Values.Add(dict.ContainsKey(attr) ? dict[attr] : "-");
                    }

                    result.Add(row);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return Enumerable.Empty<AttributeValues>();
            }            
        }

        public static ObservableCollection<MainInfoValue> GetMainInfoObservableCollection(Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {
            var content = new List<MainInfoValue>();

            foreach (var dataObject in dataObjects)
            {
                content.Add(new MainInfoValue
                {
                    DisplayName = dataObject?.DisplayName,
                    ID = dataObject?.Id,
                    Created = dataObject?.Created,
                    Creator = dataObject.Creator?.DisplayName,
                    Type = dataObject.Type?.Title,
                });
            }

            return new ObservableCollection<MainInfoValue>(content);
        }
    }
}
