using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    class SpecManager {
        /* spec
         *      text
         *      sele ( = valveSele, only sele at this depth form spec.)
         *              sele ( = boreSele, answer to bore and question stype)
         *                  spco ( answer to stype )
         *                  spco ( answer to stype )
         *              sele
         *                  spco ( answer to stype )
         *              sele
         */



        public DbElement Spec { get; private set; }
        
        public DbElement FirstSele { get {
                //valveSele must always exist
                foreach (var sele in Spec.Members().Where(m => m.ElementType == DbElementTypeInstance.SELEC)) {
                    if (sele.GetAsString(DbAttributeInstance.TANS) == FirstSeleTans) {
                        return sele;
                    }
                }

                var newSele = Spec.CreateLast(DbElementTypeInstance.SELEC);
                newSele.SetAttribute(DbAttributeInstance.TANS, FirstSeleTans);
                newSele.SetAttribute(DbAttributeInstance.QUES, "PBOR");
                newSele.SetAttribute(DbAttributeInstance.QUAL, 1);

                return newSele;

            }
        }

        public DbElement BoreSele(double bore) {

            Console.WriteLine(FirstSele.FullName() + " looking for boreSele with size: " + bore.ToString());
            DbElement boreSele = FirstSele.Members().SingleOrDefault(m => m.GetDouble(DbAttributeInstance.ANSW) == bore);

            if (boreSele == null || boreSele.IsValid == false) {
                Console.WriteLine("Adding new BORE SELE");
                boreSele = FirstSele.CreateLast(DbElementTypeInstance.SELEC);
                boreSele.SetAttribute(DbAttributeInstance.ANSW, bore);
                boreSele.SetAttribute(DbAttributeInstance.MAXA, bore);
                boreSele.SetAttribute(DbAttributeInstance.QUES, "SHOP");

                DbElement shopSele = boreSele.CreateLast(DbElementTypeInstance.SELEC);
                shopSele.SetAttribute(DbAttributeInstance.TANS, "TRUE");
                shopSele.SetAttribute(DbAttributeInstance.QUES, "STYP");
            }

            return boreSele.FirstMember();
            
        }
        public string FirstSeleTans { get; private set; }
        public SpecManager(DbElement spec, string firstSeleTans) {
            Spec = spec;
            FirstSeleTans = firstSeleTans;
        }

        public void AddSpco(SpcoRepresentation spcoRepre) {
            // TODO: implement better SPCO naming strategy
            Console.WriteLine("SpecManager.AddSpco");

            if (spcoRepre.Stype == string.Empty || spcoRepre.PBore1 == 0) {
                Console.WriteLine($"## Invalid input for:{spcoRepre.Sdte.GetString(DbAttributeInstance.DTXR)}");
                Console.WriteLine($"Stype cannot be empty and is: {spcoRepre.Stype}");
                Console.WriteLine($"Bore cannot be 0 and is : {spcoRepre.PBore1}");
                return;
            }
            var spcoName = Spec.Name() + spcoRepre.Name;
            if (spcoName.Length>50) {
                spcoName = Spec.Name() + spcoRepre.Name.Substring(Spec.Name().Length);
            }
            DBElementCollection spcos = new DBElementCollection(Spec, new AttributeRefFilter(DbAttributeInstance.CATR, spcoRepre.Scom));
            DbElement spco;
            if (spcos.Count() > 0) {
                spco = spcos.First();
                
            } else {
                spco = BoreSele(spcoRepre.PBore1).CreateLast(DbElementTypeInstance.SPCOMPONENT);
            }

            // TODO: do something much better. with the spco name. maybe stype + size + dtxr???
            try {
                spco.SetAttribute(DbAttributeInstance.NAME, spcoName);
            } catch {
                for (int i = 0; i < 100; i++) {
                    try {
                        spco.SetAttribute(DbAttributeInstance.NAME, spcoName + i.ToString());
                    } catch { }
                }
            }
            spco.SetAttribute(DbAttributeInstance.TANS, spcoRepre.Stype);
            spco.SetAttribute(DbAttributeInstance.CATR, spcoRepre.Scom);

            PMLCommander.RunPMLCommand(spco, "DETR", spcoRepre.Sdte.FullName(), out string error);
            PMLCommander.RunPMLCommand(spco, "MATXT", spcoRepre.Smte.FullName(), out   error);


        }
    
    }
}
