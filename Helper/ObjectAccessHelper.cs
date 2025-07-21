using Ascon.Pilot.SDK;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class ObjectAccessHelper
    {
        private readonly IObjectsRepository _objectsRepository;
        private readonly ObjectLoader _objectLoader;

        public ObjectAccessHelper(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _objectLoader = new ObjectLoader(objectsRepository);
        }

        public struct ObjectAcces
        {
            public ReadOnlyCollection<IAccessRecord> AccessRecord { get; set; }
            public bool isSecret { get; set; }
        }

        public async Task<IEnumerable<IAccessRecord>> GetObjectAccess(Guid currentObjectGuid)
        {
            var access = await GetObjectsAccessAsync(currentObjectGuid);

            return await GetAccessList(new Stack<ObjectAcces>(access));
        }


        private async Task<IEnumerable<IAccessRecord>> GetAccessList(Stack<ObjectAcces> accessStack)
        {
            var accessList = new List<IAccessRecord>();
            bool secretZoneEnable = false;
            var _isSecret = new List<bool>();

            while (accessStack.Count > 0)
            {
                var currentObjectAccess = accessStack.Pop();
                var accessRecords = currentObjectAccess.AccessRecord;

                if (!secretZoneEnable && currentObjectAccess.isSecret)
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
                                if (objectAccess.isSecret)
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

        private async Task<List<ObjectAcces>> GetObjectsAccessAsync(Guid objectGuid)
        {
            var accessInheritance = new List<ObjectAcces>();

            var dataObject = await _objectLoader.Load(objectGuid);
            var objectAccess = dataObject.Access2;

            accessInheritance.Add(new ObjectAcces()
            {
                AccessRecord = dataObject.Access2,
                isSecret = dataObject.IsSecret,
            });

            if (dataObject.Id != new Guid("00000001-0001-0001-0001-000000000001"))
            {
                accessInheritance.AddRange(await GetObjectsAccessAsync(dataObject.ParentId));
            }

            return accessInheritance;
        }

        public AccessLevelInfo ConvertRecordToAccesslevelInfo(IAccessRecord currentAccessRecord)
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
