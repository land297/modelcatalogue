using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Box : IBuildableConverter
    {
        public DbElementType SourceType { get { return DbElementTypeInstance.BOX; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.XLength = element.GetDouble(DbAttributeInstance.XLEN);
            size.YLength = element.GetDouble(DbAttributeInstance.YLEN);
            size.ZLength = element.GetDouble(DbAttributeInstance.ZLEN);

            var q = new DbQualifier();
            q.Add(6);
            q.wrtQualifier = element.OwnerInHierarchyOfType(DbElementTypeInstance.EQUIPMENT);

            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);
            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var orientation = element.GetOrientation(DbAttributeInstance.ORI,q);
            direction.X = orientation.XDir().ToString();
            direction.Y = orientation.YDir().ToString();
            direction.Z = orientation.ZDir().ToString();


            return new Buildable(DbElementTypeInstance.LPYRAMID, element, size, position, direction,element.ElementType);

        }
    }
}
