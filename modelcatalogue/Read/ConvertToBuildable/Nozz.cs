using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Nozz : IBuildableConverter
    {
        public DbElementType SourceType { get { return DbElementTypeInstance.NOZZLE; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();


            var q = new DbQualifier();
            q.Add(1);

            size.Diameter = element.GetDouble(DbAttributeInstance.PPBO, q);
            var coco = element.GetString(DbAttributeInstance.PPCO, q);
            var pos = element.GetPosition(DbAttributeInstance.PPOS, q);

            position.X = pos.X;
            position.Y = pos.Y;
            position.Z = pos.Z;

            var d = element.GetDirection(DbAttributeInstance.PDIR, q);
            direction.X = d.ToString();
            direction.Y = d.ToString();
            direction.Z = d.ToString();

            var catref = DbElement.GetElement();
            if (element.GetValidRef(DbAttributeInstance.CATR, ref catref)) {
                var catrefOfCatref = DbElement.GetElement();
                if (catref.GetValidRef(DbAttributeInstance.CATR, ref catrefOfCatref)) {
                    DbElement[] blrfarray = new DbElement[5];
                    if (catrefOfCatref.GetValidRefArray(DbAttributeInstance.BLRF, ref blrfarray)) {
                        Console.WriteLine(blrfarray.Count());
                    }
                }
            }


            return new Buildable(DbElementTypeInstance.NOZZLE, element, size, position, direction,coco);

        }
    }
}
