using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.Geometry;
using Aveva.Core.Utilities;
using Aveva.Core.Utilities.CommandLine;
using Aveva.Core.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class Factory
    {
        private int _ppointCounter = 5;
        private DbElement _category { get; set; }
        private DbElement _gmse { get; set; }
        private DbElement _ptse { get; set; }
        public Factory(DbElement category, DbElement p, DbElement g) {
            _category = category;
            _ptse = p;
            _gmse = g;
        }
        public DbElement PTCA(string direction, Position position) {
            var error = string.Empty;
            //TODO: check if PTCA with direction and position already exisits!
            foreach (DbElement p in new DBElementCollection(_ptse, new TypeFilter(DbElementTypeInstance.PTCAR))) {
                var pDir = p.GetAsString(DbAttributeInstance.PTCD);
                var pX = p.GetString(DbAttributeInstance.PX).Clean();
                var pZ = p.GetAsString(DbAttributeInstance.PZ).Clean();
                var pY = p.GetString(DbAttributeInstance.PY).Clean();

                if (pDir == direction &&
                    position.XString() == pX &&
                    position.YString() == pY &&
                    position.ZString() == pZ) {
                    return p;
                }

            }


            var ptca = _ptse.Create(1, DbElementTypeInstance.PTCAR);
            //doesn't compute number correct. should start at 10 and just add one...
            int number = _ppointCounter++;
            //ptca.SetAttribute(DbAttributeInstance.NUMB, number);
            RunPMLCommand2(ptca, "NUMB", number.ToString(), out error);
            

            //if (!RunPMLCommand(ptca, "PTCDIR", direction, out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PX", $"({position.XString()})", out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PY", $"({position.YString()})", out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PZ", $"({position.ZString()})", out error)) {
            //    Console.WriteLine(error);
            //}


            //SetDBExpression(ptca, position.XString(), DbAttributeInstance.PX, out var pdmserror);
            //SetDBExpression(ptca, position.YString(), DbAttributeInstance.PY, out pdmserror);
            //SetDBExpression(ptca, position.ZString(), DbAttributeInstance.PZ, out pdmserror);
            //SetDBExpression(ptca, direction, DbAttributeInstance.PTCD, out pdmserror);

            RunPMLCommand2(ptca, "PX", position.XString(), out error);
            RunPMLCommand2(ptca, "PY", position.YString(), out error);
            RunPMLCommand2(ptca, "PZ", position.ZString(), out error);
            RunPMLCommand(ptca, "PTCDI", direction, out error);


            return ptca;
        }

        public DbElement ConnectingPTCA(string direction, Position position, double size, int number, string coco) {
            var ptca = _ptse.Create(1, DbElementTypeInstance.PTCAR);

            ptca.SetAttribute(DbAttributeInstance.NUMB, number);

            var error = string.Empty;

            //SetDBExpression(ptca, position.XString(), DbAttributeInstance.PX, out var pdmserror);
            //SetDBExpression(ptca, position.YString(), DbAttributeInstance.PY, out pdmserror);
            //SetDBExpression(ptca, position.ZString(), DbAttributeInstance.PZ, out pdmserror);
            RunPMLCommand2(ptca, "PX", position.XString(), out error);
            RunPMLCommand2(ptca, "PY", position.YString(), out error);
            RunPMLCommand2(ptca, "PZ", position.ZString(), out error);


            //SetDBExpression(ptca, direction, DbAttributeInstance.PTCD, out pdmserror);

            //SetDBExpression(ptca, size.ToString(), DbAttributeInstance.PBOR, out pdmserror);
            //SetDBExpression(ptca, coco, DbAttributeInstance.PCON, out pdmserror);
            RunPMLCommand(ptca, "PBOR", size.ToString(), out error);
            RunPMLCommand(ptca, "PCON", coco, out error);

            RunPMLCommand(ptca, "PTCDI", direction, out error);
            //if (!RunPMLCommand(ptca, "PX", $"({position.XString()})", out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PY", $"({position.YString()})", out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PZ", $"({position.ZString()})", out error)) {
            //    Console.WriteLine(error);
            //}

            //if (!RunPMLCommand(ptca, "PBORE", $"{size.ToString()}", out error)) {
            //    Console.WriteLine(error);
            //}
            //if (!RunPMLCommand(ptca, "PCONN", $"{coco}", out error)) {
            //    Console.WriteLine(error);
            //}

            return ptca;
        }

        /// <summary>
        /// DO NOT USE FOR POINTSETS
        /// </summary>
        /// <param name="element"></param>
        /// <param name="expression"></param>
        /// <param name="attribute"></param>
        /// <param name="pdmserror"></param>
        /// <returns></returns>
        private bool SetDBExpression(DbElement element, string expression, DbAttribute attribute, out PdmsMessage pdmserror) {       
            if (DbExpression.Parse("'" + expression + "'", out var expParsed, out pdmserror)) {
                Console.WriteLine($"Parsed {expression} expression to: {expParsed.ToString()} {expParsed.Type.ToString()}");
                if (element.SetAttribute(attribute, new DbQualifier(), expParsed, out pdmserror)) {
                    Console.WriteLine($"SetDbExpression {attribute} {expression} on {element.Name()}/{element.ElementType.Name} successfull");
                    return true;
                } else {
                    Console.WriteLine($"SetDbExpression {attribute} {expression} on {element.Name()}/{element.ElementType.Name} fail: {pdmserror.MessageText()}");

                    //Aveva.Core.Geometry.Direction dir = Aveva.Core.Geometry.Direction.Create(expression);
                    //Console.WriteLine(dir.ToString());

                    //DbAttribute[] ttt = element.GetAttributes();
                    //foreach (DbAttribute item in ttt) {
                    //    Console.WriteLine(item.Name + " " + item.Type + " " + item.QueryText);
                    //}

                    string[] commands = new string[3];
                    commands[0] = element.Name();
                    commands[1] = $"{attribute} {expression}";

                    string filePath = Path.Combine(Path.GetTempPath(), "sidpdpcommnad.txt");
                    
                    File.WriteAllLines(filePath, commands);

                    var pmlCommand = Command.CreateCommand($"$m /{filePath}");
                    var result = pmlCommand.RunInPdms();
                   



                    return false;
                }
            } else {
                Console.WriteLine($"SetDBExpression: Could not parse {expression} - {pdmserror.MessageText()} - running as PMLCommand");
                var pmlResult = RunPMLCommand(element, attribute.Name, expression, out string error);
                if (pmlResult == false) {
                    Console.WriteLine("Could not manage with PMLCommnad: " + error);
                }
                return false;
            }

        }

        private bool RunPMLCommand(DbElement ce, string attribute, string command, out string error) {
            if (command.Length == 0) {
                error = "Command is empty";
                return false;
            }
            CurrentElement.Element = ce;
            Command pmlCommand;
            //if (attribute == "PTCDIR") {
            //Console.WriteLine("RunPMLCommand:");
            //varför de här jävla fnuttarna nu?!
            pmlCommand = Command.CreateCommand(attribute + " " + command);
            //}
            //else {
            //    pmlCommand = Command.CreateCommand($"{attribute} ({command})");
            //}
            //var result = pmlCommand.RunInPdms();
            var result = pmlCommand.RunInPdms();
            error = pmlCommand.Error.MessageText();
            //Console.WriteLine(error);
            return result;
        }
        private bool RunPMLCommand2(DbElement ce, string attribute, string command, out string error) {
            return RunPMLCommand(ce, attribute,  $"({command})", out error);
        }

        public void BOX(Direction direction, Position position, Size size) {
            LPYR(direction, position, size);
        }

        public void LPYR(Direction direction, Position position, Size size) {
            var ptcaX = PTCA(direction.X, position);
            var ptcaY = PTCA(direction.Y, position);
            var ptcaZ = PTCA(direction.Z, position);
            var lpyr = _gmse.Create(1, DbElementTypeInstance.LPYRAMID);

            lpyr.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            PdmsMessage pdmserror;
            RunPMLCommand(lpyr, "PBTP", size.XLength.ToString(), out error);
            RunPMLCommand(lpyr, "PCTP", size.YLength.ToString(), out error);
            RunPMLCommand(lpyr, "PBBT", size.XLength.ToString(), out error);
            RunPMLCommand(lpyr, "PCBT", size.YLength.ToString(), out error);
            RunPMLCommand(lpyr, "PTDI", size.ZLength.ToString(), out error);

            //SetDBExpression(lpyr, size.XLength.ToString(), DbAttributeInstance.PBTP, out pdmserror);
            //SetDBExpression(lpyr, size.YLength.ToString(), DbAttributeInstance.PCTP, out pdmserror);

            //SetDBExpression(lpyr, size.XLength.ToString(), DbAttributeInstance.PBBT, out pdmserror);
            //SetDBExpression(lpyr, size.YLength.ToString(), DbAttributeInstance.PCBT, out pdmserror);

            //SetDBExpression(lpyr, size.ZLength.ToString(), DbAttributeInstance.PTDI, out pdmserror);

            //data of wrong type
            //lpyr.SetAttribute(DbAttributeInstance.PBAX, ptcaX);
            RunPMLCommand(lpyr, "PBAX", $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(lpyr, "PCAX", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(lpyr, "PAAX", $"P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);

            SetDBExpression(lpyr, $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", DbAttributeInstance.PBAX, out pdmserror);

        }

        public void SEXT(Buildable element) {
            var ptcaX = PTCA(element.Direction.X, element.Position);
            var ptcaY = PTCA(element.Direction.Y, element.Position);
            var ptcaZ = PTCA(element.Direction.Z, element.Position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SEXTRUSION);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;

            RunPMLCommand(geom, "PAAX", $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(geom, "PBAX", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(geom, "PHEI", element.Size.Height.ToString(), out error);

            RunPMLCommand(geom, "PX", element.Position.X.ToString(), out error);
            RunPMLCommand(geom, "PY", element.Position.Y.ToString(), out error);
            RunPMLCommand(geom, "PZ", element.Position.Z.ToString(), out error);

            var sloo = geom.Create(1, DbElementTypeInstance.SLOOP);

            foreach (var vert in element.Verticies) {
                var svert = sloo.CreateLast(DbElementTypeInstance.SVERTEX);
                RunPMLCommand(svert, "PX", vert.X.ToString(), out error);
                RunPMLCommand(svert, "PY", vert.Y.ToString(), out error);

            }

        }

        public void SDSH(Direction direction, Position position, Size size) {
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SDSH);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);

            SetDBExpression(geom, size.XLength.ToString(), DbAttributeInstance.PDIA, out var pdmserror);
            geom.SetAttribute(DbAttributeInstance.PHEI, size.YLength);
            geom.SetAttribute(DbAttributeInstance.PRAD, size.ZLength.ToString());

            geom.SetAttribute(DbAttributeInstance.PAAX, ptca);
        }

        public void SCYL(Direction direction, Position position, Size size) {
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SCYLINDER);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            RunPMLCommand(geom, "PHEI", size.Height.ToString(), out error);

            //SetDBExpression(geom, size.Diameter.ToString(), DbAttributeInstance.PDIA, out var pdmserror);
            //SetDBExpression(geom, size.Height.ToString(), DbAttributeInstance.PHEI, out pdmserror);
            //geom.SetAttribute(DbAttributeInstance.PHEI, size.Height);

            RunPMLCommand(geom, "PAXIS", $"-P{ptca.GetAsString(DbAttributeInstance.NUMB)}", out error);
            //geom.SetAttribute(DbAttributeInstance.PAAX, ptca);
        }

        public void LSNO(Direction direction, Position position, Size size) {
            var ptcaY = PTCA(direction.Y, position);
            var ptcaZ = PTCA(direction.Z, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.LSNOUT);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            geom.SetAttribute(DbAttributeInstance.CLFL, false);

            //geom.SetAttribute(DbAttributeInstance.PBDI, size.ZLength);
            RunPMLCommand(geom, "PTDI", size.Height.ToString(), out var error);
            //invert top and bot in paragon...
            RunPMLCommand(geom, "PTDM", size.BotDiameter.ToString(), out error);
            RunPMLCommand(geom, "PBDM", size.TopDiameter.ToString(), out error);
            RunPMLCommand(geom, "POFF","0", out error);
            //SetDBExpression(geom, size.Height.ToString(), DbAttributeInstance.PTDI, out var pdmserror);
            
            //SetDBExpression(geom, size.BotDiameter.ToString(), DbAttributeInstance.PTDM, out pdmserror);
            //SetDBExpression(geom, size.TopDiameter.ToString(), DbAttributeInstance.PBDM, out pdmserror);
            //SetDBExpression(geom, "0", DbAttributeInstance.POFF, out pdmserror);
            //RunPMLCommand(geom, "Pbdistance", $"{size.ZLength}", out string error);
            //geom.SetAttribute(DbAttributeInstance.PTDM, (int)size.XLength);
            //geom.SetAttribute(DbAttributeInstance.PBDM, size.YLength);
            //geom.SetAttribute(DbAttributeInstance.POFF, 0);

            RunPMLCommand(geom, "PAAXIS", $"-P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(geom, "PBAXIS", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);

        }

        public void Ppoint(Direction direction, Position position, Size size, int number, string coco) {
            var ptca = ConnectingPTCA(direction.Z, position, size.Diameter, number, coco);
        }



        public DbElement CreateOrUpdateSDTE(DbElement equipment) {
            var skey = string.Empty;
            var dtxr = string.Empty;

            foreach (DbElement text in new DBElementCollection(equipment.Owner, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Purpose() == "DTXT") {
                    dtxr = text.GetString(DbAttributeInstance.STEX);
                }
                if (text.Purpose() == "SKEY") {
                    skey = text.GetString(DbAttributeInstance.STEX);
                }
            }

            return SDTE(dtxr, skey);
        }

        private DbElement SDTE(string detailText, string skey) {
            DbElement sdte = _category.Members().SingleOrDefault(m => m.ElementType == DbElementTypeInstance.SDTEXT);
            if (sdte == null || sdte.IsValid == false) {
                sdte = _category.CreateLast(DbElementTypeInstance.SDTEXT);
            }
            RunPMLCommand(sdte, "SKEY", $"'{skey}'", out var error);
            Console.WriteLine(error);
            RunPMLCommand(sdte, "RTEXT", $"'{detailText}'", out error);
            Console.WriteLine(error);
            //if (sdte.SetAttributeValue(DbAttributeInstance.SKEY, skey) == false) {
            //    Console.WriteLine("FAIL Set SKEY");
            //}
            //if (!sdte.SetAttributeValue(DbAttributeInstance.RTEX, detailText)) {
            //    Console.WriteLine("FAIL SET RTEXT");

            //}

            return sdte;
        }

        public static bool ValidSkey(DbElement equipment) {
            var skey = string.Empty;
            foreach (DbElement text in new DBElementCollection(equipment, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Name().Split('/').Last() == "SKEY") {
                    skey = text.GetString(DbAttributeInstance.STEX);
                }
            }
            return skey == string.Empty ? false : true;
        }
        public static bool ValidDtxr(DbElement equipment) {
            var dtxr = string.Empty;
            foreach (DbElement text in new DBElementCollection(equipment, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Name().Split('/').Last() == "DETAILTEXT") {
                    dtxr = text.GetString(DbAttributeInstance.STEX);
                }
            }
            return dtxr == string.Empty ? false : true;
        }

        public void Create(Buildable element) {
            //var m1 = GetType().GetMethods();
            //var m2 = GetType().GetMethod("SCYL");

            //var type = DesignToParagonTypeTranslator.Translate(element.ElementType);
            //var method = GetType().GetMethod(type);


            //method.Invoke(this, new object[] { element.Direction, element.Position, element.Size });

            //replace the DesignToParagonTypeTranslator with this in 
            Create(element, 0);
        }
        public void Create(Buildable element, int number) {

            switch (element.ElementType) {
                case "EXTR":
                    SEXT(element);
                    break;
                case "BOX":
                    LPYR(element.Direction, element.Position, element.Size);
                    break;
                case "CYLI":
                    SCYL(element.Direction, element.Position, element.Size);
                    break;
                case "CONE":
                    LSNO(element.Direction, element.Position, element.Size);
                    break;
                case "NOZZ":
                    Ppoint(element.Direction, element.Position, element.Size, number, element.Coco);
                    break;
                default: break;
            }
        }
        //public void CreateP1andP2(DbElement equipment) {
        //    DBElementCollection nozzles = new DBElementCollection(equipment, new TypeFilter(DbElementTypeInstance.NOZZLE));
        //    int index = 1;
        //    foreach (DbElement nozzle in nozzles) {
        //        DbElement catref = default(DbElement);
        //        if (nozzle.GetValidRef(DbAttributeInstance.CATR, ref catref)) {
        //            // TODO: number definitions..
        //            //var number = int.Parse(nozzle.Name().Split('/').Last().Replace("P", string.Empty));
        //            //always read P1 och nozzle for the position and direction and bore
        //            var nozzleP1 = ReadModel.Read.ReadElement(nozzle, 1);
        //            //use parsed number to create
        //            Create(nozzleP1, index++);


        //        }

        //    }
        //}
    }
}
