using Ascon.Pilot.SDK;
using MathNet.Numerics.Distributions;
using MyIceLibrary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIceLibrary.Helper
{
    public class AccessModifier
    {
        private readonly ObjectLoader _objectLoader;
        private readonly IObjectModifier _objectModifier;

        public AccessModifier(IObjectsRepository objectsRepository, IObjectModifier objectModifier)
        {
            _objectLoader = new ObjectLoader(objectsRepository);
            _objectModifier = objectModifier;
        }


        public async Task AddAccessToObjectAsync(IAccessRecord accessRecord, Guid targetObject)
        {
            var objectInfo = await _objectLoader.Load(targetObject);

            var builder = _objectModifier.EditById(objectInfo.Id);

            builder.AddAccessRecords(
                accessRecord.OrgUnitId,
                accessRecord.Access.AccessLevel,
                accessRecord.Access.ValidThrough,
                accessRecord.Access.Inheritance,
                accessRecord.Access.Type,
                accessRecord.Access.TypeIds);

            _objectModifier.Apply();
        }

        public async Task RemoveAccessFromObjectAsync(AccessCheckBox[] accessItems, Guid currentObjectGuid)
        {
            foreach (var accessRecord in accessItems)
            {
                if (accessRecord.IsSelected)
                {
                    await RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, currentObjectGuid);
                }
            }
        }

        public async Task RemoveAccessFromParentObjectsAsync(AccessCheckBox[] accessItems, ParentInfo[] parents, Guid currentObjectGuid, bool deleteAccessInCurrentObject)
        {
            if (deleteAccessInCurrentObject)
            {
                await RemoveAccessFromObjectAsync(accessItems, currentObjectGuid);
            }    

            await RemoveAccessOnParentAsync(accessItems, parents);
        }

        private async Task RemoveAccessOnParentAsync(AccessCheckBox[] accessItems, ParentInfo[] parents)
        {
            try
            {
                foreach (var accessRecord in accessItems)
                {
                    if (accessRecord.IsSelected)
                    {
                        foreach (var parent in parents)
                        {
                            var findUnit = await IsObjectHasOrgUnitIdAsync(accessRecord.AccessRecord.OrgUnitId, parent.ObjectGuid);

                            if (findUnit.unitFind &&
                                accessRecord.AccessRecord.Access.AccessLevel == findUnit.unitAccessRecord.Access.AccessLevel)
                            {
                                await RemoveAccessRecordByOrgUnitIdAsync(accessRecord.AccessRecord, parent.ObjectGuid);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошиюка: {ex}");
            }
            finally
            {
                System.Windows.MessageBox.Show($"Права успешно удалены");
            }
        }

        public async Task RemoveAccessRecordByOrgUnitIdAsync(IAccessRecord accessRecord, Guid targetObject)
        {
            var objectInfo = await _objectLoader.Load(targetObject);

            var builder = _objectModifier.EditById(objectInfo.Id);

            builder.RemoveAccessRecords(x => RemoveAccessRecordFunc(x, accessRecord.OrgUnitId));

            _objectModifier.Apply();
        }

        private bool RemoveAccessRecordFunc(IAccessRecord accessRecord, int targetOrgUnitId)
        {
            bool result = false;

            if (accessRecord.OrgUnitId == targetOrgUnitId)
            {
                result = true;
            }

            return result;
        }

        public async Task<(bool unitFind, IAccessRecord unitAccessRecord)> IsObjectHasOrgUnitIdAsync(int orgUnitId, Guid objectGuid)
        {
            var dataObject = await _objectLoader.Load(objectGuid);
            var accessRecords = dataObject.Access2;

            bool unitFind = false;
            IAccessRecord accessRecord = null;

            foreach (var record in accessRecords)
            {
                if (record.OrgUnitId == orgUnitId)
                {
                    unitFind = true;
                    accessRecord = record;

                    break;
                }
            }

            return (unitFind, accessRecord);
        }
    }
}
