using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Read
{
    public class Reader
    {
        public ConvertToBuildable.Convert Converter { get; set; }
        public IEnumerable<Buildable> GetBuildables(DbElement owner) {
            TypeFilter box = new TypeFilter(DbElementTypeInstance.BOX);
            TypeFilter cone = new TypeFilter(DbElementTypeInstance.CONE);
            TypeFilter cylinder = new TypeFilter(DbElementTypeInstance.CYLINDER);
            TypeFilter dish = new TypeFilter(DbElementTypeInstance.DISH);
            TypeFilter extr = new TypeFilter(DbElementTypeInstance.EXTRUSION);
            TypeFilter nozz = new TypeFilter(DbElementTypeInstance.NOZZLE);
            TypeFilter ctorus = new TypeFilter(DbElementTypeInstance.CTORUS);

            OrFilter orFilter = new OrFilter(new BaseFilter[] { box, cone, cylinder,
                dish, extr,nozz,ctorus });

            DBElementCollection elements = new DBElementCollection(owner, orFilter);
            foreach (DbElement element in elements) {
                yield return Converter.ElementToBuildable(element);
            }
        }
    }
}
