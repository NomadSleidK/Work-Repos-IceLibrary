using Ascon.Pilot.SDK;
using MyIceLibrary.Extensions;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class AccessLoader
    {
        private readonly IObjectsRepository _objectsRepository; 
        private readonly ObjectLoader _objectLoader;

        private readonly Dictionary<AccessLevel, string> _accessNames;

        public AccessLoader(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _objectLoader = new ObjectLoader(objectsRepository);

            _accessNames = new Dictionary<AccessLevel, string>()
            {
                { AccessLevel.None, "Нет доступа" },
                { AccessLevel.Create, "Создание" },
                { AccessLevel.Edit, "Редактирование атрибутов и файлов" },
                { AccessLevel.View, "Просмотр" },
                { AccessLevel.Freeze, "Заморозка" },
                { AccessLevel.Agreement, "Согласование" },
                { AccessLevel.Share, "Делегирование прав доступа" },
                { AccessLevel.ViewCreate, "" },
                { AccessLevel.ViewEdit, "" },
                { AccessLevel.ViewEditAgrement, "" },
                { AccessLevel.Full, "Полный доступ" },
            };
        }

        public string GetAccessNameByEnum(AccessLevel level)
        {
            string result = "";

            foreach (AccessLevel value in Enum.GetValues(typeof(AccessLevel)))
            {
                if (value != AccessLevel.None && (level & value) == value)
                {
                    if (_accessNames[value] != "")
                    {
                        result += _accessNames[value] + ", ";
                    }
                }
            }

            result = result.Substring(0, result.Length - 2);

            return result;
        }

        public struct ObjectAccess
        {
            public ReadOnlyCollection<IAccessRecord> AccessRecord { get; set; }
            public bool IsSecret { get; set; }
        }

        public async Task<IEnumerable<IAccessRecord>> GetObjectAccess(Guid currentObjectGuid)
        {
            var access = await GetAccessFromAllObjectsAsync(currentObjectGuid);

            return await GetAccessForCurrentObject(new Stack<ObjectAccess>(access));
        }

        private async Task<IEnumerable<IAccessRecord>> GetAccessForCurrentObject(Stack<ObjectAccess> accessStack)
        {
            var accessList = new List<IAccessRecord>();
            bool secretZoneEnable = false;

            while (accessStack.Count > 0)
            {
                var currentObjectAccess = accessStack.Pop();
                var accessRecords = currentObjectAccess.AccessRecord;

                if (!secretZoneEnable && currentObjectAccess.IsSecret)
                {
                    secretZoneEnable = true;
                }

                foreach (var record in accessRecords)
                {
                    if (record.Access.Inheritance == AccessInheritance.InheritUntilSecret)
                    {
                        if (!secretZoneEnable)
                        {
                            bool secretObjectFind = false;

                            foreach (var objectAccess in accessStack)
                            {
                                if (objectAccess.IsSecret)
                                {
                                    secretObjectFind = true;
                                    break;
                                }
                            }

                            if (!secretObjectFind)
                            {
                                accessList.Add(record);
                            }
                        }
                        else
                        {
                            accessList.Add(record);
                        }

                    }
                    else if (record.Access.Inheritance == AccessInheritance.InheritWholeSubtree)
                    {
                        accessList.Add(record);
                    }
                    else if (record.Access.Inheritance == AccessInheritance.None && accessStack.Count == 0)
                    {
                        accessList.Add(record);
                    }
                }
            }

            return accessList;
        }

        private async Task<List<ObjectAccess>> GetAccessFromAllObjectsAsync(Guid objectGuid)
        {
            var accessInheritance = new List<ObjectAccess>();

            var dataObject = await _objectLoader.Load(objectGuid);

            accessInheritance.Add(new ObjectAccess()
            {
                AccessRecord = dataObject.Access2,
                IsSecret = dataObject.IsSecret,
            });

            if (!dataObject.Id.IsRoot())
            {
                accessInheritance.AddRange(await GetAccessFromAllObjectsAsync(dataObject.ParentId));
            }

            return accessInheritance;
        }

        public AccessLevelInfo ConvertAccessRecordToAccessLevelInfo(IAccessRecord currentAccessRecord)
        {
            var organizationUnit = _objectsRepository.GetOrganisationUnit(currentAccessRecord.OrgUnitId);
            var accessInfo = new AccessLevelInfo()
            {
                PersonName = organizationUnit.Title,
                None = false,
                Create = false,
                Edit = false,
                View = false,
                Freeze = false,
                Agreement = false,
                Share = false,
                ViewCreate = false,
                ViewEdit = false,
                ViewEditAgrement = false,
                Full = false
            };

            if (organizationUnit.Kind() == OrganizationUnitKind.Position && organizationUnit.Person() != -1)
            {
                accessInfo.PersonName = _objectsRepository.GetPerson(organizationUnit.Person()).DisplayName;
            }

            AccessLevel level = currentAccessRecord.Access.AccessLevel;

            foreach (AccessLevel value in Enum.GetValues(typeof(AccessLevel)))
            {
                if (value != AccessLevel.None && (level & value) == value)
                {
                    switch (value)
                    {
                        case AccessLevel.None:
                            accessInfo.None = true;
                            break;
                        case AccessLevel.Create:
                            accessInfo.Create = true;
                            break;
                        case AccessLevel.Edit:
                            accessInfo.Edit = true;
                            break;
                        case AccessLevel.View:
                            accessInfo.View = true;
                            break;
                        case AccessLevel.Freeze:
                            accessInfo.Freeze = true;
                            break;
                        case AccessLevel.Agreement:
                            accessInfo.Agreement = true;
                            break;
                        case AccessLevel.Share:
                            accessInfo.Share = true;
                            break;
                        case AccessLevel.ViewCreate:
                            accessInfo.ViewCreate = true;
                            break;
                        case AccessLevel.ViewEdit:
                            accessInfo.ViewEdit = true;
                            break;
                        case AccessLevel.ViewEditAgrement:
                            accessInfo.ViewEditAgrement = true;
                            break;
                        case AccessLevel.Full:
                            accessInfo.Full = true;
                            break;
                    }
                }
            }

            return accessInfo;
        }
    }
}
