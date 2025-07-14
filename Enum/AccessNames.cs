using Ascon.Pilot.SDK;
using System.Collections.Generic;

namespace MyIceLibrary.AccessNames
{
    public static class AccessNames
    {
        private static Dictionary<AccessLevel, string> _names = new Dictionary<AccessLevel, string>()
        {
            { AccessLevel.None, "Нет доступа" },
            { AccessLevel.Create, "Создавать" },
            { AccessLevel.Edit, "Редактировать" },
            { AccessLevel.View, "Просматривать" },
            { AccessLevel.Freeze, "Заморозка" },
            { AccessLevel.Agreement, "Согласование" },
            { AccessLevel.Share, "Делегирование прав доступа" },
            { AccessLevel.ViewCreate, "Создание и просмотр" },
            { AccessLevel.ViewEdit, "Редактирование и просмотр" },
            { AccessLevel.ViewEditAgrement, "Редактирование, просмотр, Делегирование прав доступа" },
            { AccessLevel.Full, "Полный доступ" },
        };

        public static string GetAccessName(AccessLevel level) => _names[level];
    }
}
