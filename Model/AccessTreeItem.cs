using Ascon.Pilot.SDK;
using System.Collections.Generic;

namespace MyIceLibrary.Model
{
    public class AccessTreeItem
    {
        public object Name { get; set; }
        public List<AccessTreeItem> Children { get; set; } = new List<AccessTreeItem>();
        public IOrganisationUnit Unit { get; set; }
        public bool IsExpanded { get; set; }
    }
}
