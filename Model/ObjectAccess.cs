using Ascon.Pilot.SDK;
using System.Collections.ObjectModel;

namespace MyIceLibrary.Model
{
    public struct ObjectAccess
    {
        public ReadOnlyCollection<IAccessRecord> AccessRecord { get; set; }
        public bool IsSecret { get; set; }
    }
}