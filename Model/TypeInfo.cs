namespace MyIceLibrary.Model
{
    public struct TypeInfo
    {
        public byte[] Icon { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Kind { get; set; }
        public bool IsMountable { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsProject { get; set; }
        public bool IsService { get; set; }
    }
}