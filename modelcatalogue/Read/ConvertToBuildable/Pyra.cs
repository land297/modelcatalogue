using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Pyra : IBuildableConverter
    {
        public DbElementType SourceType { get { return DbElementTypeInstance.PYRAMID; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.XBottom = element.GetDouble(DbAttributeInstance.XBOT);
            size.YBottom = element.GetDouble(DbAttributeInstance.YBOT);
            size.XTop = element.GetDouble(DbAttributeInstance.XTOP);
            size.YTop = element.GetDouble(DbAttributeInstance.YTOP);
            size.Height = element.GetDouble(DbAttributeInstance.HEIG);
            size.XOffset = element.GetDouble(DbAttributeInstance.XOFF);
            size.YOffset = element.GetDouble(DbAttributeInstance.YOFF);

            var q = new DbQualifier();
            q.Add(2);
            q.wrtQualifier = element.OwnerInHierarchyOfType(DbElementTypeInstance.EQUIPMENT);

            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);
            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var orientation = element.GetOrientation(DbAttributeInstance.ORI,q);
            direction.X = orientation.XDir().ToString();
            direction.Y = orientation.YDir().ToString();
            direction.Z = orientation.ZDir().ToString();


            return new Buildable(DbElementTypeInstance.LPYRAMID, element, size, position, direction, element.ElementType);

        }
    }
}
