using System.Collections.Generic;

namespace MyIceLibrary.Model
{
    public class TreeItem
    {
        public object Name { get; set; }
        public List<TreeItem> Children { get; set; } = new List<TreeItem>();
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
    }
}
