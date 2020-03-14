using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aveva.Core.Database;

namespace modelcatalogue.Create
{
    //class CateModifier
    //{
    //    private DbElement _scom;
    //    private ReadModel.Hierarchy _hierarchyReader = new ReadModel.Hierarchy();
    //    public CateModifier(DbElement scom) {
    //        _scom = scom;
    //    }
    //    internal void Update(DbElement equipment) {
    //        foreach (DbElement member in _scom.Owner.Members()) {
    //            if (member.ElementType != DbElementTypeInstance.SCOMPONENT 
    //                && member.ElementType != DbElementTypeInstance.SDTEXT) {
    //                member.Delete();
    //            }
    //        }

    //        DbElement ptse = _scom.Owner.Create(1, DbElementTypeInstance.PTSET);
    //        DbElement gmse = _scom.Owner.Create(1, DbElementTypeInstance.GMSET);
    //        DbElement dtse = _scom.Owner.Create(1, DbElementTypeInstance.DTSET);

    //        _scom.SetAttribute(DbAttributeInstance.PTRE, ptse);
    //        _scom.SetAttribute(DbAttributeInstance.GMRE, gmse);
    //        _scom.SetAttribute(DbAttributeInstance.DTRE, dtse);

    //        var creator = new Create.Factory(_scom.Owner, ptse, gmse);
    //        foreach (DbElement e in _hierarchyReader.BuildableElementsInEquipment(equipment)) {
    //            var x = ReadModel.Read.ReadElement(e);
    //            creator.Create(x);
    //        }

    //        creator.CreateP1andP2(equipment);
    //        creator.CreateOrUpdateSDTE(equipment);
    //    }
    //}
}
