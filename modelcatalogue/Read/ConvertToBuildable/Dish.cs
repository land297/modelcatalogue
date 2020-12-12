using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Dish : IBuildableConverter
    {

        public DbElementType SourceType { get { return DbElementTypeInstance.DISH; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();



            var v1 = element.GetDouble(DbAttributeInstance.DIAM);
            size.ZLength = element.GetDouble(DbAttributeInstance.RADI);
 
            size.Height = element.GetDouble(DbAttributeInstance.HEIG);
            size.Diameter = element.GetDouble(DbAttributeInstance.DIAM);

            var q = new DbQualifier();
            q.Add(2);
            q.wrtQualifier = element.OwnerInHierarchyOfType(DbElementTypeInstance.EQUIPMENT);
            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);

            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var dir = element.GetDirection(DbAttributeInstance.PDIR, q).Opposite();

            direction.X = dir.ToString();
            direction.Y = dir.ToString();
            direction.Z = dir.ToString();

 
            return new Buildable(DbElementTypeInstance.SDSH, element, size, position, direction, element.ElementType);

        }
    
    }
}
