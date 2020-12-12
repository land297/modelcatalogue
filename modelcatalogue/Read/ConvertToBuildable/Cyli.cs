using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Cyli : IBuildableConverter
    {
        public DbElementType SourceType { get { return DbElementTypeInstance.CYLINDER; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.Height = element.GetDouble(DbAttributeInstance.HEIG);
            size.Diameter = element.GetDouble(DbAttributeInstance.DIAM);

            var q = new DbQualifier();
            q.Add(1);
            q.wrtQualifier = element.OwnerInHierarchyOfType(DbElementTypeInstance.EQUIPMENT);
            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);
            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var d = element.GetDirection(DbAttributeInstance.PDIR, q);
            direction.X = d.ToString();
            direction.Y = d.ToString();
            direction.Z = d.ToString();


            return new Buildable(DbElementTypeInstance.SCYLINDER, element, size, position, direction, element.ElementType);

        }
    }
}
