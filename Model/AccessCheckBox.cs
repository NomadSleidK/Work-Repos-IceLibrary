using Ascon.Pilot.SDK;

namespace MyIceLibrary.Model
{
    public class AccessCheckBox
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IAccessRecord AccessRecord { get; set; }
        public bool IsSelected { get; set; }
    }
}
