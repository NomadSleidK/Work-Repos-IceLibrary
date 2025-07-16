namespace MyIceLibrary.Model
{
    public class AccessLevelInfo
    {
        public object PersonName { get; set; }
        public bool None { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Freeze { get; set; }
        public bool Agreement { get; set; }
        public bool Share { get; set; }
        public bool ViewCreate { get; set; }
        public bool ViewEdit { get; set; }
        public bool ViewEditAgrement { get; set; }
        public bool Full { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            AccessLevelInfo other = (AccessLevelInfo)obj;
            return PersonName == other.PersonName;
        }

        public override int GetHashCode()
        {
            return PersonName.GetHashCode();
        }

    }
}
