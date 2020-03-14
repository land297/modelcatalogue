using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aveva.Core.Database;

namespace modelcatalogue.Create
{
    public class Cate
    {
        public DbElement Element { get; private set; }
        internal DbElement CreateEmptyScom(DbElement cata, DbElement equipment) {
            //look for correct SECT
            DbElement sect = cata.Members().SingleOrDefault(s => s.GetString(DbAttributeInstance.DESC) == equipment.Owner.GetString(DbAttributeInstance.DESC));
            if (sect == null || sect.IsValid == false) {
                sect = cata.CreateLast(DbElementTypeInstance.SECTION);
                sect.SetAttributeValue(DbAttributeInstance.DESC, equipment.Owner.GetString(DbAttributeInstance.DESC));
            }

            DbElement cate = sect.Create(1, DbElementTypeInstance.CATEGORY);
            
            DbElement sdte = cate.Create(1, DbElementTypeInstance.SDTEXT);
            DbElement ptse = cate.Create(1, DbElementTypeInstance.PTSET);
            DbElement gmse = cate.Create(1, DbElementTypeInstance.GMSET);
            DbElement dtse = cate.Create(1, DbElementTypeInstance.DTSET);

            DbElement scom = cate.Create(1, DbElementTypeInstance.SCOMPONENT);

            scom.SetAttribute(DbAttributeInstance.PTRE, ptse);
            scom.SetAttribute(DbAttributeInstance.GMRE, gmse);
            scom.SetAttribute(DbAttributeInstance.DTRE, dtse);

            scom.SetAttribute(DbAttributeInstance.DESC, equipment.Name());
            scom.SetAttribute(DbAttributeInstance.NAME, equipment.Name() + ".SCOM");

            Element = cate;
            return scom;
        }

        internal DbElement Update(DbElement equi, DbElement scom) {
            var cate = scom.Owner;
            
            foreach (var member in cate.Members()) {
                if (member.ElementType == DbElementTypeInstance.PTSET ||
                    member.ElementType == DbElementTypeInstance.GMSET ||
                    member.ElementType == DbElementTypeInstance.DTSET) {
                    member.Delete();
                }
            }

            DbElement ptse = cate.Create(1, DbElementTypeInstance.PTSET);
            DbElement gmse = cate.Create(1, DbElementTypeInstance.GMSET);
            DbElement dtse = cate.Create(1, DbElementTypeInstance.DTSET);


            scom.SetAttribute(DbAttributeInstance.PTRE, ptse);
            scom.SetAttribute(DbAttributeInstance.GMRE, gmse);
            scom.SetAttribute(DbAttributeInstance.DTRE, dtse);

            Element = cate;
            return scom;
        }
    }
}
