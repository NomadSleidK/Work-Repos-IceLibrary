using Ascon.Pilot.SDK;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class ObjectsTreeBuilder
    {
        private readonly IObjectsRepository _objectsRepository;

        public ObjectsTreeBuilder(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
        }

        public async Task<ObservableCollection<TreeItem>> CreateObjectTreeButtomToTopAsync(IDataObject dataObject, ObservableCollection<TreeItem> mainTreeItems)
        {
            TreeItem treeItem;

            if (mainTreeItems != null)
            {
                treeItem = mainTreeItems[0];

                mainTreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject,
                        IsExpanded = true, IsSelected = false, Children = new List<TreeItem>() { treeItem } }
                };
            }
            else
            {
                mainTreeItems = new ObservableCollection<TreeItem>
                {
                    new TreeItem { Name = dataObject.DisplayName, DataObject = dataObject,
                        IsExpanded = true, IsSelected = false, Children = null }
                };
            }

            if (dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001")) //Начало 00000001-0001-0001-0001-000000000001

            {
                ObjectLoader objectLoader = new ObjectLoader(_objectsRepository);

                var dataObjects = await objectLoader.Load(dataObject.ParentId);
                mainTreeItems = await CreateObjectTreeButtomToTopAsync(dataObjects, mainTreeItems);
            }

            return mainTreeItems;
        }

        public async Task<ObservableCollection<TreeItem>> FilteredTreeItemsAsync(string parameter, ObservableCollection<TreeItem> originTreeItems)
        {
            var treeItems = new ObservableCollection<TreeItem>();

            string input = parameter;

            if (input != "")
            {
                List<TreeItem> items = originTreeItems.DeepCopy().ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var result = await CheckTreeElementsAsync(items[i], input);
                    items[i] = result.treeItem;
                }

                treeItems = new ObservableCollection<TreeItem>(items);
            }
            else
            {
                treeItems = new ObservableCollection<TreeItem>(originTreeItems.DeepCopy().ToList());
            }

            return treeItems;
        }

        private async Task<(TreeItem treeItem, bool isFind)> CheckTreeElementsAsync(TreeItem currentItem, string input)
        {
            bool isFind = false;

            var children = currentItem.Children;

            if (currentItem.Name.ToLower().Contains(input.ToLower()))
            {
                isFind = true;
                currentItem.IsSelected = true;
            }
            else
            {
                currentItem.IsSelected = false;
            }

            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        var childInfo = await CheckTreeElementsAsync(children[i], input);

                        if (childInfo.isFind)
                        {
                            isFind = true;
                        }
                        else
                        {
                            currentItem.Children.Remove(children[i]);
                            i--;
                        }
                    }
                    else
                    {
                        currentItem.Children.Remove(children[i]);
                        i--;
                    }
                }
            }


            return (currentItem, isFind);
        }

        public ObservableCollection<TreeItem> CreateOrganisationUnitTreeTopToButtomAsync()
        {
            var unitsId = _objectsRepository.GetOrganisationUnit(0).Children;
            var units = new List<IOrganisationUnit>();

            foreach (var id in unitsId)
            {
                units.Add(_objectsRepository.GetOrganisationUnit(id));
            }

            var tree = new List<TreeItem>();

            foreach (var unit in units)
            {
                tree.Add(BuildTreeItem(unit));
            }

            return new ObservableCollection<TreeItem>(tree);
        }

        private TreeItem BuildTreeItem(IOrganisationUnit unit)
        {
            var item = new TreeItem
            {
                Name = unit.Title,
                IsExpanded = true,
                Children = new List<TreeItem>(),
                DataObject = unit,
            };

            if (unit.Kind() == OrganizationUnitKind.Position)
            {
                item.Children.Add(new TreeItem()
                {
                    Name = _objectsRepository.GetPerson(unit.Person()).DisplayName,
                    IsExpanded = true,
                    DataObject = unit,
                    Children = null
                });
            }

            foreach (var childId in unit.Children)
            {
                var childUnit = _objectsRepository.GetOrganisationUnit(childId);
                if (childUnit != null)
                {
                    item.Children.Add(BuildTreeItem(childUnit));
                }
            }

            return item;
        }

        public ObservableCollection<TreeItem> CreateSnapshotsTree(IFilesSnapshot[] filesSnapshots)
        {
            var treeItems = new List<TreeItem>();

            foreach (var fileSnapshot in filesSnapshots)
            {
                var newTreeItem = new TreeItem()
                {
                    Name = fileSnapshot.Created.ToString("dd-MM-yyyy HH:mm:ss"),
                    DataObject = fileSnapshot,
                    Children = new List<TreeItem>(),
                    IsExpanded = true,
                    IsSelected = false,
                };

                var files = fileSnapshot.Files;

                foreach (var file in files)
                {
                    newTreeItem.Children.Add(new TreeItem()
                    {
                        Name = file.Name,
                        DataObject = file,
                        Children = null,
                        IsExpanded = true,
                        IsSelected = false,
                    });
                }
                treeItems.Add(newTreeItem);
            }

            return new ObservableCollection<TreeItem>(treeItems);
        }
    }
}
