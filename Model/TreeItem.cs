using Ascon.Pilot.SDK;
using System.Collections.Generic;

namespace MyIceLibrary.Model
{
    public class TreeItem
    {
        public object Name { get; set; }
        public List<TreeItem> Children { get; set; } = new List<TreeItem>();
        public IDataObject DataObject { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
    }
}
