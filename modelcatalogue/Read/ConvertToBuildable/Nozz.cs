using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Nozz : IBuildableConverter {
        public DbElementType SourceType { get { return DbElementTypeInstance.NOZZLE; } }
        public Buildable Convert(DbElement element) {
            Size size = new Size();
            Position position = new Position();
            Direction direction = new Direction();


            var q = new DbQualifier();
            q.Add(1);
            var nozzleConfig = new NozzleConfig();
            size.Diameter = element.GetDouble(DbAttributeInstance.PPBO, q);
            nozzleConfig.Coco = element.GetString(DbAttributeInstance.PPCO, q);
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
                        nozzleConfig.Blrfarray = blrfarray;
                    }
                    
                    var attributes = catrefOfCatref.GetAttributes();
                    foreach (DbAttribute item in attributes) {
                        Console.WriteLine(item.Description);
                    }


                    nozzleConfig.Parameters = catrefOfCatref.GetDoubleArray(DbAttributeInstance.PARA);

                    var dataset = DbElement.GetElement();
                    if (catrefOfCatref.GetValidRef(DbAttributeInstance.DTRE, ref dataset)) {
                        nozzleConfig.Dataset = dataset;
                    } 
                    //if (!dataset.IsValid) {
                    //    var flanScom = MDB.CurrentMDB.FindElement("/EMFWBR0-40-0");
                    //    if (flanScom.GetValidRef(DbAttributeInstance.DTRE, ref dataset)) {
                    //        Console.WriteLine(dataset);
                    //        nozzleConfig.Dataset = dataset;
                    //    }
                    //}
                }
            }

            var name = element.Name().ToUpper();
            if (name.Contains("ARRIVE")) {
                nozzleConfig.Ppoint = 1;
            } else if (name.Contains("LEAVE")) {
                nozzleConfig.Ppoint = 2;
            } else if (name.Contains("P#")) {
                if (int.TryParse(name.Split('#').Last(), out int p)) {
                    nozzleConfig.Ppoint = p;
                }
            }
            return new Buildable(DbElementTypeInstance.NOZZLE, element, size, position, direction, nozzleConfig);
        }
    }

    public class NozzleConfig {
        public string Coco { get; set; }
        public DbElement[] Blrfarray { get; set; }
        public int Ppoint { get; set; } = -1;

        public DbElement Dataset { get; set; }
        public double[] Parameters { get; set; }
        //public 
    }
}
