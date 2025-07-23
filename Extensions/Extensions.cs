using System;

namespace MyIceLibrary.Extensions
{
    public static class Extensions
    {
        public static bool IsRoot(this Guid guid) => guid == new Guid("00000001-0001-0001-0001-000000000001");
        public static bool IsEmpty(this Guid guid) => guid == Guid.Empty;      
    }
}
