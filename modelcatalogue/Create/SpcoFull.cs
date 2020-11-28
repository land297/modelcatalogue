using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class SpcoFull
    {
        private DbElement _equipment;
        //public string SpecName { get; set; }
        public double PBore1 { get; private set; }
        public double PBore2 { get; private set; }
        public DbElement Sdte { get; private set; }
        public DbElement Scom { get; private set; }
        private string _stype;
        public string Stype { get { return _stype ?? _equipment.Owner.GetString(DbAttributeInstance.FUNC); } }
        public string FirstSeleTans { get; private set; } 
        public string Skey { get; private set; }
        public string Name { get { return Scom.Name().Replace(".SCOM", string.Empty) + ".SPCO"; } }

        public SpcoFull(DbElement scom, DbElement equipment, string stype, string skey) : this(scom,equipment){
            _stype = stype;
            Skey = skey;
        }

        public SpcoFull(DbElement scom, DbElement equipment) {
            Scom = scom;
            _equipment = equipment;
            Sdte = scom.Owner.Members().Single(m => m.ElementType == DbElementTypeInstance.SDTEXT);
            DBElementCollection ptcas = new DBElementCollection(Scom.Owner, new TypeFilter(DbElementTypeInstance.PTCAR));
            foreach (DbElement ptca in ptcas) {
                if (ptca.GetAsString(DbAttributeInstance.NUMB) == "1") {
                    var t = ptca.GetAsString(DbAttributeInstance.PBOR);
                    t = t.Replace("(", string.Empty);
                    t = t.Replace("'", string.Empty);
                    t = t.Replace(")", string.Empty);
                    PBore1 = double.Parse(t);

                } else if (ptca.GetAsString(DbAttributeInstance.NUMB) == "2") {
                    var t = ptca.GetAsString(DbAttributeInstance.PBOR);
                    t = t.Replace("(", string.Empty);
                    t = t.Replace("'", string.Empty);
                    t = t.Replace(")", string.Empty);
                    PBore2 = double.Parse(t);
                }
            }

            //TODO: replace function
            var purp = _equipment.Owner.GetString(DbAttributeInstance.PURP);
            if (purp.Length < 4) {
                FirstSeleTans = "VALV";
            } else {
                FirstSeleTans = purp ;
            }
        }

        
    }
}
