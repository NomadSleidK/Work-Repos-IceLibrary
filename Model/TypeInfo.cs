namespace MyIceLibrary.Model
{
    public struct TypeInfo
    {
        public byte[] Icon { get; set; }
        public object Name { get; set; }
        public object Id { get; set; }
        public object Title { get; set; }
        public object Kind { get; set; }
        public object IsMountable { get; set; }
        public object IsDeleted { get; set; }
        public object IsProject { get; set; }
        public object IsService { get; set; }
    }
}