using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Extr : IBuildableConverter
    {
        
        public DbElementType SourceType { get { return DbElementTypeInstance.EXTRUSION; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();

            size.Height = element.GetDouble(DbAttributeInstance.HEIG);

            var pos = element.GetPosition(DbAttributeInstance.POS);
            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var orientation = element.GetOrientation(DbAttributeInstance.ORI);
            direction.X = orientation.XDir().ToString();
            direction.Y = orientation.YDir().ToString();
            direction.Z = orientation.ZDir().ToString();

            var vertPos = new List<Position>();
            foreach (var vert in element.FirstMember().Members()) {
                pos = vert.GetPosition(DbAttributeInstance.POS);
                var vp = new Position();
                vp.X = pos.X;
                vp.Y = pos.Y;
                vp.Z = pos.Z;

                vertPos.Add(vp);
            }

            return new Buildable(DbElementTypeInstance.SEXTRUSION,element, size, position, direction, vertPos);

        }
    }
}
