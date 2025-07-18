using Ascon.Pilot.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyIceLibrary.Model
{
    public class TreeItem
    {
        public string Name { get; set; }
        public List<TreeItem> Children { get; set; } = new List<TreeItem>();
        public object DataObject { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public TreeItem DeepCopy()
        {
            var copy = new TreeItem
            {
                Name = this.Name,
                DataObject = this.DataObject,
                IsExpanded = this.IsExpanded
            };

            if (Children != null)
            {
                foreach (var child in this.Children)
                {
                    copy.Children.Add(child.DeepCopy());
                }
            }

            return copy;
        }
    }

    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<TreeItem> DeepCopy<TreeItem>(this ObservableCollection<TreeItem> original)
            where TreeItem : class
        {
            var copy = new ObservableCollection<TreeItem>();
            
            if (original != null)
            {
                foreach (var item in original)
                {
                    if (item is ICloneable cloneable)
                    {
                        copy.Add((TreeItem)cloneable.Clone());
                    }
                    else if (item.GetType().GetMethod("DeepCopy") != null)
                    {
                        var deepCopyMethod = item.GetType().GetMethod("DeepCopy");
                        var copiedItem = deepCopyMethod.Invoke(item, null);
                        copy.Add((TreeItem)copiedItem);
                    }
                    else
                    {
                        copy.Add(item);
                    }
                }
            }         

            return copy;
        }
    }
}