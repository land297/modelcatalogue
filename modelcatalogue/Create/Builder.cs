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

        internal void SetCata(DbElement cata) {
            Cata = cata;
        }

        public DbElement BuildScom(DbElement equi) {
            DbElement scom = DbElement.GetElement();

            if (DoesScomExsist(equi.Name(), ref scom)) {
                Cate.Update(equi, scom);
            } else {
                scom = Cate.CreateEmptyScom(Cata, equi);
            }

            var buildables = Reader.GetBuildables(equi);
            var geometry = new Geometry(scom);

            geometry.AddGeometry(buildables);

            var text = new Text();
            text.CreateOrUpdateSDTE(equi, Cate.Element);


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
