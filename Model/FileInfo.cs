using System;

namespace MyIceLibrary.Model
{
    public struct FileInfo
    {
        public Guid Id {  get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Accessed { get; set; }
        public long Size { get; set; }
        public string Md5 { get; set; }
    }
}
