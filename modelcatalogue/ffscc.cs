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
            return "0.3";
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
    }
}
