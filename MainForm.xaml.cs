using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyIceLibrary
{
    public partial class MainForm : Window
    {
        public delegate void AccountHandler(IEnumerable<Guid> selectedGuids);
        public event AccountHandler OnDeleteButtonClick;

        private readonly bool _isAdmin;

        public MainForm(bool isAdmin)
        {
            InitializeComponent();
            _isAdmin = isAdmin;

            DeleteButton.IsEnabled = _isAdmin;
            DeleteButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Buttons Click
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            var rows = MainInfoDataGrid.SelectedItems;
            var selectedGuids = new List<Guid>();

            foreach (var selectedItem in rows)
            {
                string result = ((MainInfoValue)selectedItem).ID.ToString();

                if (Guid.TryParse(result.ToString(), out Guid guid))
                {
                    selectedGuids.Add(guid);
                }
            }

            OnDeleteButtonClick?.Invoke(selectedGuids);


            //var rows = MainInfoDataGrid.SelectedItems;
            //List<Guid> selectedGuids = new List<Guid>();

            //foreach (var selectedItem in rows)
            //{
            //    string result = ((MainInfoValue)selectedItem).ID.ToString();

            //    if (Guid.TryParse(result.ToString(), out Guid guid))
            //    {
            //        selectedGuids.Add(guid);
            //    }
            //}


            //foreach (var obj in selectedGuids)
            //{
            //    MessageBox.Show($"Объект на удаление c Guid = {obj} ");
            //    _modifier.DeleteById(obj);
            //    _modifier.Apply();
            //}
        }
        #endregion

        #region Load Attributes Objects ToGrid
        public void LoadAttributesObjectsToGrid(Ascon.Pilot.SDK.IDataObject[] objects)
        {
           var attributes = new List<IDictionary<string, object>>();

            foreach (var obj in objects)
            {
                attributes.Add(obj.Attributes);
            }

            LoadAttributeToGrid(AttributesDataGrid, attributes.ToArray());
        }

        private void LoadAttributeToGrid(DataGrid dataGrid, IDictionary<string, object>[] attributes)
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

        private IEnumerable<AttributeValues> BringTogetherAllAttributes(IDictionary<string, object>[] inputData)
        {
            // Собираем все уникальные атрибуты из всех словарей
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

                // Для каждого словаря ищем значение атрибута
                foreach (var dict in inputData)
                {
                    row.Values.Add(dict.ContainsKey(attr) ? dict[attr] : "-");
                }

                result.Add(row);
            }

            return result;
        }
        #endregion

        #region Load Main Info To Grid
        public void LoadMainInfoToGrid(Ascon.Pilot.SDK.IDataObject[] dataObjects)
        {
            var content = new List<MainInfoValue>();
            //DeleteButton.on
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

            LoadMainInfoToGrid(MainInfoDataGrid, content);
        }

        private void LoadMainInfoToGrid(DataGrid dataGrid, IEnumerable<MainInfoValue> rows)
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
        #endregion
    }
}