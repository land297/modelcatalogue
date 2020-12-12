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
            TypeFilter pyramid = new TypeFilter(DbElementTypeInstance.PYRAMID);

            OrFilter orFilter = new OrFilter(new BaseFilter[] { box, cone, cylinder,
                dish, extr,nozz,ctorus,pyramid });

            DBElementCollection elements = new DBElementCollection(owner, orFilter);
            foreach (DbElement element in elements) {
                yield return Converter.ElementToBuildable(element);
            }
        }

        public int GetLastSessionModified(DbElement root) {
            int session = root.GetInteger(DbAttributeInstance.SESSM);

            return LastSessionInHierarchy(root, session);
        }

        private int LastSessionInHierarchy(DbElement root, int session) {
            foreach (var member in root.Members()) {
                var memberSession = member.GetInteger(DbAttributeInstance.SESSM);
                if (memberSession > session) {
                    session = memberSession;
                }
                memberSession = LastSessionInHierarchy(member,session);
                if (memberSession > session) {
                    session = memberSession;
                }
            }
            return session;
        }
    }
}
