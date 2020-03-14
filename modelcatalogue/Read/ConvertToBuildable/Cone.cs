using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Cone : IBuildableConverter
    {

        public DbElementType SourceType { get { return DbElementTypeInstance.CONE; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.TopDiameter = element.GetDouble(DbAttributeInstance.DTOP);
            size.BotDiameter = element.GetDouble(DbAttributeInstance.DBOT);
            size.Height = element.GetDouble(DbAttributeInstance.HEIG);

            var q = new DbQualifier();
            q.Add(1);
            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);

            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var orientation = element.GetOrientation(DbAttributeInstance.ORI);
            direction.X = orientation.XDir().ToString();
            direction.Y = orientation.YDir().ToString();
            direction.Z = orientation.ZDir().ToString();


            return new Buildable(DbElementTypeInstance.LSNOUT, element, size, position, direction);

        }

    }
}
