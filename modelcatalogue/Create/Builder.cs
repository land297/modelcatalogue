using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aveva.Core.Database;
using modelcatalogue.Read;

namespace modelcatalogue.Create
{
    public class Builder
    {
        public DbElement Cata { get; set; }
        public Cate Cate { get; set; }
        public Reader Reader { get; set; }
        public Text Text { get; private set; }

        internal void SetCata(DbElement cata) {
            Cata = cata;
        }

        public DbElement BuildScom(DbElement equi) {
            DbElement scom = DbElement.GetElement();

            if (DoesScomExsist(equi.Name(), ref scom)) {
                Cate.Update(scom,equi);
            } else {
                scom = Cate.CreateEmptyScom(Cata, equi);
            }

            var buildables = Reader.GetBuildables(equi);
            var geometry = new Geometry(scom);

            geometry.AddGeometry(buildables);

            Text = new Text();
            Text.CreateOrUpdateSDTE(equi, Cate.Element);


            // need to save first
            // and then loop through all others, and see if they do not match, and update to new
            // parameter and stuff..
            // foreach nozzleconfig
            var builableWithnozzleConfig = buildables.FirstOrDefault(b => b.NozzleConfig != null);
            if (builableWithnozzleConfig != null ) {
                DbElement dataset = DbElement.GetElement();
                scom.GetValidRef(DbAttributeInstance.DTRE, ref dataset);

                if (builableWithnozzleConfig.NozzleConfig.Dataset != null && builableWithnozzleConfig.NozzleConfig.Dataset.IsValid) {
                    dataset.CopyHierarchy(builableWithnozzleConfig.NozzleConfig.Dataset, new DbCopyOption());
                }
                scom.SetAttribute(DbAttributeInstance.PARA, builableWithnozzleConfig.NozzleConfig.Parameters);
               
                if (builableWithnozzleConfig.NozzleConfig.Coco.Substring(0, 1) == "F") {
                    scom.SetAttribute(DbAttributeInstance.BLRF, builableWithnozzleConfig.NozzleConfig.Blrfarray);
                } else {
                    scom.SetAttributeDefault(DbAttributeInstance.BLRF);
                }

            } else {

            }

            return scom;
        }

        private bool DoesScomExsist(string equipmentName, ref DbElement scom) {
            var scomName = equipmentName + ".SCOM";
            var possibleElement = MDB.CurrentMDB.FindElement(DbType.Catalogue, scomName);
            if (possibleElement.IsValidEx()) {
                scom = possibleElement;
                return true;
            }
            return false;
        }
    }
}
