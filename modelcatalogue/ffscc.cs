using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.PMLNet;
using Aveva.Core.Utilities;
using Aveva.Core.Utilities.CommandLine;
using Aveva.Core.Utilities.Messaging;
using modelcatalogue.Create;
using modelcatalogue.Read;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
//using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace modelcatalogue
{
    [PMLNetCallable()]
    public class ffscc {

        [PMLNetCallable()]
        public ffscc() {
            //try {
            //    byte[] b = File.ReadAllBytes(@"C:\Program Files (x86)\AVEVA\Everything3D2.10\TemplatePMLNET.dll");
            //    Assembly a = Assembly.Load(b);

            //    var t = a.GetType("TemplatePMLNET.NamespaceForClarity.Class");
            //    var tt = Activator.CreateInstance(t);
            //    var m = t.GetMethod("Execute");
            //    m.Invoke(tt, null);
            //} catch { }
        }

        [PMLNetCallable()]
        public void Assign(ffscc that) {
        }
        [PMLNetCallable()]
        public string GetVersion() {
            return "0.2";
        }

        [PMLNetCallable()]
        public void Include(string equipmentName, string specNaem) {
            try {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var builder = new Builder();
                var spec = MDB.CurrentMDB.FindElement(DbType.Catalog, specNaem);
                var equi = MDB.CurrentMDB.FindElement(DbType.Design, equipmentName);

                builder.Cata = Cata.GetWritable(equi);
                builder.Cate = new Cate();
                builder.Reader = new Reader();
                builder.Reader.Converter = new ConvertToBuildable.Convert();

                var scom = builder.BuildScom(equi);
                var session = builder.Reader.GetLastSessionModified(equi);
                Console.WriteLine($" sessionmod : {session}");
                var spcoInfo = new Create.SpcoRepresentation(scom, equi, builder.Text);
                spcoInfo.DefineMatxt(builder.Cata);

                var specManager = new Create.SpecManager(spec, spcoInfo.FirstSeleTans);
                specManager.AddSpco(spcoInfo);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
            }
        }

        //[PMLNetCallable()]
        //public void Execute(){
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");


        //    // make/locate cata we can write to
        //    var cataName = "/sidp.dp.0.1";
        //    Db[] dbs = MDB.CurrentMDB.GetDBArray();
        //    DbElement cata = MDB.CurrentMDB.FindElement(DbType.Catalog, cataName);
        //    Db db = default(Db);
        //    if (!cata.IsValid) {
        //        //find cata dbs with dbwrite eq true
        //        DbElement e = MDB.CurrentMDB.FindElement(cataName);
        //        if (e.IsValid) {
        //            Console.WriteLine(e.Name());
        //        }
        //        db = dbs.FirstOrDefault(d => d.Type == DbType.Catalog && !d.IsReadOnly);

        //        Console.WriteLine($"can write to:{ db.Number} - { db.IsReadOnly}- { db.Type}");
        //        cata = db.World.Create(1, DbElementTypeInstance.CATALOGUE);
        //        cata.SetAttribute(DbAttributeInstance.NAME, cataName);
        //    } else {
        //        db = cata.Db;
        //    }

        //    var hierachyReader = new ReadModel.Hierarchy();

        //    Dictionary<DbElement, DbElement> scomsEquipments = new Dictionary<DbElement, DbElement>();
        //    Console.WriteLine("Going to look for zones");
        //    foreach (DbElement zone in hierachyReader.ElementCollectionOfZones()) {
        //        Console.WriteLine($"zone: {zone.FullName()}");
        //        foreach (DbElement equipment in hierachyReader.GetEquipmentsInZone(zone)) {
        //            Console.WriteLine($"equipment: {equipment.FullName()}");
        //            if (Create.Factory.ValidDtxr(equipment) == false) {
        //                Console.WriteLine("Invalid Dtxr, will not create");
        //                continue;
        //            }
        //            if (Create.Factory.ValidSkey(equipment) == false) {
        //                Console.WriteLine("Invalid Dtxr, will not create");
        //                continue;
        //            }
        //            // 1 - check if we have exisiting CATE for this
        //            BaseFilter descValueFilter = new AttributeStringFilter(DbAttributeInstance.DESC, FilterOperator.Equals, equipment.Name());
        //            DBElementCollection scoms = new DBElementCollection(cata, new AndFilter(new TypeFilter(DbElementTypeInstance.SCOMPONENT), descValueFilter));
        //            if (scoms.Count()>0) {
        //                DbElement scom = default(DbElement);
        //                foreach (DbElement item in scoms) {
        //                    scom = item;
        //                    break;
        //                }
        //                Console.WriteLine(scom.FullName());
        //                var cateModifier = new Create.CateModifier(scom);
        //                cateModifier.Update(equipment);
        //                scomsEquipments.Add(scom, equipment);
        //            } else {
        //                var cateCreator = new Create.Cate(cata);
        //                DbElement scom = cateCreator.Create(equipment);
        //                scom.SetAttribute(DbAttributeInstance.DESC, equipment.Name());
        //                scomsEquipments.Add(scom, equipment);
        //            }
        //            Console.WriteLine("----------------------------------------");
        //        }
        //    }

        //    // spec-part. name, desc of site?
        //    var specWldName = "/sidp.spwld.0.2";
        //    DbElement specWld = MDB.CurrentMDB.FindElement(DbType.Catalog, specWldName);

        //    if (!specWld.IsValid) {
        //        specWld = db.World.Create(1, DbElementTypeInstance.SPWLD);
        //        specWld.SetAttribute(DbAttributeInstance.NAME, specWldName);
        //        specWld.SetAttribute(DbAttributeInstance.PURP, "PIPE");

        //    }

        //    // key = scom, value = equipment
        //    foreach (var kvp in scomsEquipments) {
        //        var spcoInfo = new Create.SpcoFull(kvp.Key, kvp.Value);
        //        var specName = kvp.Value.Owner.Owner.Name() + specWldName;
        //        DbElement spec = MDB.CurrentMDB.FindElement(DbType.Catalog, specName);
        //        if (spec.IsValid == false) {
        //            var specManager = new Create.SpecManager(specWld, specName);
        //            specManager.AddSpco(spcoInfo);

        //        } else {
        //            var specManager = new Create.SpecManager(spec);
        //            specManager.AddSpco(spcoInfo);
        //        }
        //    }

            ////spec is valid. see what we can find.

            //)

            //if (!specWld.IsValid) {
            //    specWld = db.World.Create(1, DbElementTypeInstance.SPWLD);
            //    specWld.SetAttribute(DbAttributeInstance.NAME, specWldName);

            //    spec = specWld.Create(1, DbElementTypeInstance.SPECIFICATION);
            //    spec.SetAttribute(DbAttributeInstance.NAME, specName);
            //    spec.SetAttribute(DbAttributeInstance.QUES, "TYPE");

            //    valveSele = spec.CreateLast(DbElementTypeInstance.SELEC);
            //    valveSele.SetAttribute(DbAttributeInstance.TANS, "VALV");
            //    valveSele.SetAttribute(DbAttributeInstance.QUES, "PBOR");
            //} else {
            //    spec = MDB.CurrentMDB.FindElement(DbType.Catalog, specName);
            //    if (!spec.IsValid) {
            //        spec = specWld.Create(1, DbElementTypeInstance.SPECIFICATION);
            //        spec.SetAttribute(DbAttributeInstance.NAME, specName);
            //        spec.SetAttribute(DbAttributeInstance.QUES, "TYPE");

            //        valveSele = spec.CreateLast(DbElementTypeInstance.SELEC);
            //        valveSele.SetAttribute(DbAttributeInstance.TANS, "VALV");
            //        valveSele.SetAttribute(DbAttributeInstance.QUES, "PBOR");
            //    }

            //    valveSele = spec.FirstMember();
            //}
            //BaseFilter filter = new AttributeDoubleFilter(DbAttributeInstance.ANSW, FilterOperator.Equals, double.Parse(spco.PBore1.ToString()));
            //TypeFilter tfilter = new TypeFilter(DbElementTypeInstance.SELEC);

            //DBElementCollection sele = new DBElementCollection(spec, new AndFilter(filter,tfilter));

            //DbElement pboreSele = default(DbElement);
            //bool needNewPborSele = true;
            //Console.WriteLine("Seles from filter:");
            //foreach (DbElement dbE in sele) {

            //    Console.WriteLine(dbE.FullName() + " - " + dbE.ElementType.ToString());
            //    needNewPborSele = false;
            //    pboreSele = dbE;
            //}

            //if (needNewPborSele) {
            //    pboreSele = valveSele.CreateLast(DbElementTypeInstance.SELEC);
            //    pboreSele.SetAttribute(DbAttributeInstance.ANSW, spco.PBore1);
            //    pboreSele.SetAttributeValue(DbAttributeInstance.QUES, "STYP");


            //}
            //Console.WriteLine("va faan");
            //Console.WriteLine(pboreSele.ElementType.ToString());
            //filter = new AttributeRefFilter(DbAttributeInstance.CATR, FilterOperator.Equals, spco.Scom);

            //DbElement dbSpco = default(DbElement);
            //bool newScom = true;
            //foreach (DbElement item in new DBElementCollection(pboreSele,filter)) {
            //    newScom = false;
            //    dbSpco = item;
            //}

            //if (newScom) {
            //    dbSpco = pboreSele.CreateLast(DbElementTypeInstance.SPCOMPONENT);
            //}

            //Console.WriteLine($"SPCO NAME: {dbSpco.FullName()}");

            //dbSpco.SetAttribute(DbAttributeInstance.CATR, spco.Scom);

            ////foreach (var item in dbSpco.GetAttributes()) {
            ////    Console.WriteLine(item.Name);
            ////}
            //Console.WriteLine(spco.Sdte.ElementType);
            //Console.WriteLine(spco.Sdte.FullName());
            //DbExpression dbExp = default(DbExpression);
            //var s = $"DETR {spco.Sdte.FullName()}";
            //Console.WriteLine($"command {s}");
            //CurrentElement.Element = dbSpco;
            //var pmlCommand = Command.CreateCommand(s);
            //pmlCommand.RunInPdms();
            ////DbExpression.Parse(s, out dbExp, out var pdmserror);

            ////Console.WriteLine($"eror: { pdmserror.MessageText()}");
            ////Console.WriteLine(dbExp.Type.ToString());
            ////Console.WriteLine("...");
            ////dbSpco.SetAttribute(DbAttributeInstance.DETR, new DbQualifier(), dbExp, out pdmserror);

            ////dbSpco.SetAttribute(DbAttributeInstance.DETR, spco.Sdte);


        //}

        //foreach (DbElement cylinder in stru.Cylinders()) {
        //    var cylElement = ReadModel.Read.ReadElement(cylinder);
        //    creator.SCYL(cylElement.Direction, cylElement.Position, cylElement.Size);
        //}
    
    }
}
