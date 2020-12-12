using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aveva.Core.Database;
using Aveva.Core.Database.Filters;

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
            DbElement scom = cate.Create(1, DbElementTypeInstance.SCOMPONENT);
            scom.SetAttribute(DbAttributeInstance.NAME, equipment.Name() + ".SCOM");
            DbElement sdte = cate.Create(1, DbElementTypeInstance.SDTEXT);
            Update(scom,equipment);

            Element = cate;
            return scom;
        }

        internal DbElement Update(DbElement scom, DbElement equipment) {
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

            scom.SetAttribute(DbAttributeInstance.DESC, equipment.Name());

            var possbileTypes = new DBElementCollection(equipment,
                new AndFilter(new TypeFilter(DbElementTypeInstance.TEXT),
                  new AttributeStringFilter(DbAttributeInstance.PURP, FilterOperator.Equals, "TYPE")));

            var text = possbileTypes.First();
            if (text != null && text.IsValid) {
                scom.SetAttribute(DbAttributeInstance.GTYP, text.GetAsString(DbAttributeInstance.STEX));
            } else {
                scom.SetAttribute(DbAttributeInstance.GTYP, "VALV");
            }

            Element = cate;
            return scom;
        }
    }
}
