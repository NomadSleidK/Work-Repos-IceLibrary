using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyIceLibrary
{
    public static class DataGridHelper
    {
        #region Load Attributes Objects ToGrid

        public delegate void OpenNewWindowHandler(Guid objectGuid);

        public static void LoadAttributeToGrid(DataGrid dataGrid, IDictionary<string, object>[] attributes)
        {
            try
            {
                dataGrid.Columns.Clear();

                var content = BringTogetherAllAttributes(attributes);

                // Добавляем столбец с именами атрибутов
                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Attribute",
                    Binding = new Binding("AttributeName"),
                    IsReadOnly = true,
                    Width = 250
                });

                // Добавляем столбцы
                for (int i = 0; i < attributes.Length; i++)
                {
                    var column = new DataGridTextColumn
                    {
                        Header = attributes[i]["name"] as string,
                        Binding = new Binding($"Values[{i}]"),
                        Width = 250
                    };
                    dataGrid.Columns.Add(column);
                }

                dataGrid.ItemsSource = content;
            }
            catch
            {

            }     
        }

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
            catch
            {
                return Enumerable.Empty<AttributeValues>();
            }            
        }
        #endregion

        #region Load Main Info To Grid
        public static void LoadMainInfoToGrid(DataGrid dataGrid, Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {

            try
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

                LoadMainInfoToGrid(dataGrid, content); // переименовать
            }
            catch
            {

            }
        }

        public static void LoadMainInfoToGrid(DataGrid dataGrid, IEnumerable<MainInfoValue> rows)
        {
            try
            {
                dataGrid.Columns.Clear();

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "DisplayName",
                    Binding = new Binding("DisplayName"),
                    IsReadOnly = true,
                    Width = 250
                });

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "ID",
                    Binding = new Binding("ID"),
                    IsReadOnly = true,
                    Width = 250
                });

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Created",
                    Binding = new Binding("Created"),
                    IsReadOnly = true,
                    Width = 250
                });

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Creator",
                    Binding = new Binding("Creator"),
                    IsReadOnly = true,
                    Width = 250
                });

                dataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Type",
                    Binding = new Binding("Type"),
                    IsReadOnly = true,
                    Width = 250
                });

                dataGrid.ItemsSource = rows;
            }
            catch
            {

            }       
        }
        #endregion
    }
}
