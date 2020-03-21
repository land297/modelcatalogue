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

        public void AddSpco(SpcoFull spcoInfo) {
            Console.WriteLine("SpecManager.AddSpco");
            if (spcoInfo.Stype == string.Empty || spcoInfo.PBore1 == 0) {
                Console.WriteLine($"Invalid input for:{spcoInfo.Sdte.GetString(DbAttributeInstance.DTXR)}");
                Console.WriteLine($"Stype: {spcoInfo.Stype}");
                Console.WriteLine($"Bore: {spcoInfo.PBore1}");
                return;
            }
            DBElementCollection spcos = new DBElementCollection(Spec, new AttributeRefFilter(DbAttributeInstance.CATR, spcoInfo.Scom));
            DbElement spco = default(DbElement);
            if (spcos.Count() > 0) {
                spco = spcos.First();
                spco.SetAttribute(DbAttributeInstance.NAME, Spec.Name() + spcoInfo.Name);
            } else {
                spco = BoreSele(spcoInfo.PBore1).CreateLast(DbElementTypeInstance.SPCOMPONENT);
                spco.SetAttribute(DbAttributeInstance.NAME, Spec.Name() + spcoInfo.Name);
            }
            spco.SetAttribute(DbAttributeInstance.TANS, spcoInfo.Stype);
            spco.SetAttribute(DbAttributeInstance.CATR, spcoInfo.Scom);

            //somehow cannot set detr via setAttribute...
            var commandString = $"DETR {spcoInfo.Sdte.FullName()}";
            CurrentElement.Element = spco;
            var pmlCommand = Command.CreateCommand(commandString);
            pmlCommand.RunInPdms();

            
        }
    
    }
}
