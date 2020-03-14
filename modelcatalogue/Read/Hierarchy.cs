using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ReadModel {
    public class Hierarchy {
        public IEnumerable<DbElement> GetAllCylinders() {
            DBElementCollection zones = ElementCollectionOfZones();
            TypeFilter equiFilter = new TypeFilter(DbElementTypeInstance.EQUIPMENT);
            foreach (DbElement zone in zones) {
                DBElementCollection equipments = new DBElementCollection(zone, equiFilter);
                foreach (DbElement equi in equipments) {
                    DBElementCollection cylinders = new DBElementCollection(equi, new TypeFilter(DbElementTypeInstance.CYLINDER));
                    foreach(DbElement cyli in cylinders) {
                        yield return cyli;
                    }
                }
            }
        }
        public IEnumerable<DbElement> GetEquipmentsInZone(DbElement zone) {
            TypeFilter equipment = new TypeFilter(DbElementTypeInstance.EQUIPMENT);
            DBElementCollection elements = new DBElementCollection(zone, equipment);
            foreach (DbElement element in elements) {
                yield return element;
            }
        }
        public IEnumerable<DbElement> BuildableElementsInEquipment(DbElement equipment) {
            TypeFilter box = new TypeFilter(DbElementTypeInstance.BOX);
            TypeFilter cone = new TypeFilter(DbElementTypeInstance.CONE);
            TypeFilter cylinder = new TypeFilter(DbElementTypeInstance.CYLINDER);
            TypeFilter dish = new TypeFilter(DbElementTypeInstance.DISH);
            TypeFilter extr = new TypeFilter(DbElementTypeInstance.EXTRUSION);

            OrFilter orFilter = new OrFilter(new BaseFilter[] { box, cone, cylinder, dish,extr });
            DBElementCollection elements = new DBElementCollection(equipment, orFilter);
            foreach (DbElement element in elements) {
                yield return element;
            }
        }
        
        public DBElementCollection ElementCollectionOfZones() {
            TypeFilter zoneFilter = new TypeFilter(DbElementTypeInstance.ZONE);
            AttributeStringFilter valueFilter = new AttributeStringFilter(DbAttributeInstance.FUNC, FilterOperator.Equals, "modelcatalogue");

            AndFilter af = new AndFilter(zoneFilter, valueFilter);

            DBElementCollection zones = new DBElementCollection(DbType.Design, af);
            foreach (DbElement item in zones) {
                Console.WriteLine(item.GetAsString(DbAttributeInstance.FUNC));
            }
            Console.WriteLine(zones.Count());
            return zones;
        }

    }
}
