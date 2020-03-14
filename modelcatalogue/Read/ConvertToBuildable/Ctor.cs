using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Ctor : IBuildableConverter
    {
        public DbElementType SourceType { get { return DbElementTypeInstance.CTORUS; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.Diameter = element.GetDouble(DbAttributeInstance.ROUT) - element.GetDouble(DbAttributeInstance.RINS);

            var q = new DbQualifier();
            q.Add(1);
            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);
            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var d = element.GetDirection(DbAttributeInstance.PDIR, q);
            direction.X = d.ToString();
            direction.Y = d.ToString();
            direction.Z = d.ToString();

            //TODO: fix mess
            q = new DbQualifier();
            q.Add(2);
            Position position2 = new Position();
            Direction direction2 = new Direction();

            pos = element.GetPosition(DbAttributeInstance.PPOS, q);
            position2.X = pos.X;
            position2.Y = pos.Y;
            position2.Z = pos.Z;

            d = element.GetDirection(DbAttributeInstance.PDIR, q);
            direction2.X = d.ToString();
            direction2.Y = d.ToString();
            direction2.Z = d.ToString();




            return new Buildable(DbElementTypeInstance.SCTORUS, element, size, position, direction,position2,direction2);

        }
    }
}
