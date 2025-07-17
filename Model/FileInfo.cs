using System;

namespace MyIceLibrary.Model
{
    public struct FileInfo
    {
        public Guid Id {  get; set; }
        public object Name { get; set; }
        public object Created { get; set; }
        public object Accessed { get; set; }
        public object Size { get; set; }
        public object Md5 { get; set; }
    }
}
