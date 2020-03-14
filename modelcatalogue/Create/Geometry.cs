using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class Geometry
    {
        private int _ppointCounter = 5;
        private int _nozzlePppointCounter = 1;
        private DbElement _cate { get; set; }
        private DbElement _gmse { get; set; }
        private DbElement _ptse { get; set; }
        private DbElement _scom;
        private Dictionary<DbElementType, Action<Buildable>> _createMapping = new Dictionary<DbElementType, Action<Buildable>>();
        public Geometry(DbElement scom) {
            _cate = scom.Owner;
            _gmse = scom.GetElement(DbAttributeInstance.GMRE);
            _ptse = scom.GetElement(DbAttributeInstance.PTRE);
            _scom = scom;

            //TODO: add reflection
            _createMapping.Add(DbElementTypeInstance.SEXTRUSION, SEXT);
            _createMapping.Add(DbElementTypeInstance.LPYRAMID, LPYR);
            _createMapping.Add(DbElementTypeInstance.SDSH, SDSH);
            _createMapping.Add(DbElementTypeInstance.SCYLINDER, SCYL);
            _createMapping.Add(DbElementTypeInstance.LSNOUT, LSNO);
            _createMapping.Add(DbElementTypeInstance.NOZZLE, ConnetingPPOINT);
            _createMapping.Add(DbElementTypeInstance.SCTORUS, SCTO);


        }

        public void AddGeometry(IEnumerable<Buildable> builables) {
            foreach (Buildable e in builables.Where(b => b != null)) {
                if (_createMapping.ContainsKey(e.BuildAs)) {
                    _createMapping[e.BuildAs].Invoke(e);
                }
            }
        }

        public void SEXT(Buildable element) {
            var ptcaX = PTCA(element.Direction.X, element.Position);
            var ptcaY = PTCA(element.Direction.Y, element.Position);
            //var ptcaZ = PTCA(element.Direction.Z, element.Position);
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
        public void LPYR(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptcaX = PTCA(direction.X, position);
            var ptcaY = PTCA(direction.Y, position);
            var ptcaZ = PTCA(direction.Z, position);
            var lpyr = _gmse.Create(1, DbElementTypeInstance.LPYRAMID);

            lpyr.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            RunPMLCommand(lpyr, "PBTP", size.XLength.ToString(), out error);
            RunPMLCommand(lpyr, "PCTP", size.YLength.ToString(), out error);
            RunPMLCommand(lpyr, "PBBT", size.XLength.ToString(), out error);
            RunPMLCommand(lpyr, "PCBT", size.YLength.ToString(), out error);
            RunPMLCommand(lpyr, "PTDI", size.ZLength.ToString(), out error);


            RunPMLCommand(lpyr, "PBAX", $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(lpyr, "PCAX", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(lpyr, "PAAX", $"P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(lpyr, "PAAX", $"P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        public void SDSH(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SDSH);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);

            var error = string.Empty;
            RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            RunPMLCommand(geom, "PHEI", size.Height.ToString(), out error);
            RunPMLCommand(geom, "PRAD", size.ZLength.ToString(), out error);
            RunPMLCommand(geom, "PAXI", $"P{ptca.GetAsString(DbAttributeInstance.NUMB)}", out error);

            //geom.SetAttribute(DbAttributeInstance.PHEI, size.YLength);
            //geom.SetAttribute(DbAttributeInstance.PRAD, size.ZLength.ToString());
            //geom.SetAttribute(DbAttributeInstance.PAAX, ptca);
        }
        public void SCTO(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Direction direction2 = element.Direction2;
            Position position2 = element.Position2;

            Size size = element.Size;
            var ptca1 = PTCA(direction.X, position);
            var ptca2 = PTCA(direction2.X, position2);
            var geom = _gmse.Create(1, DbElementTypeInstance.SCTORUS);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);

            var error = string.Empty;
            RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            RunPMLCommand(geom, "PAAX", $"P{ptca1.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(geom, "PBAX", $"P{ptca2.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        public void SCYL(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SCYLINDER);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            RunPMLCommand(geom, "PHEI", size.Height.ToString(), out error);
            RunPMLCommand(geom, "PAXIS", $"-P{ptca.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        public void LSNO(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptcaY = PTCA(direction.Y, position);
            var ptcaZ = PTCA(direction.Z, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.LSNOUT);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            geom.SetAttribute(DbAttributeInstance.CLFL, false);

            RunPMLCommand(geom, "PTDI", size.Height.ToString(), out var error);
            //invert top and bot in paragon...
            RunPMLCommand(geom, "PTDM", size.BotDiameter.ToString(), out error);
            RunPMLCommand(geom, "PBDM", size.TopDiameter.ToString(), out error);
            RunPMLCommand(geom, "POFF", "0", out error);


            RunPMLCommand(geom, "PAAXIS", $"-P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
            RunPMLCommand(geom, "PBAXIS", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        public void ConnetingPPOINT(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            //TODO: handle which is 1 and 2
            ConnectingPTCA(direction.Z, position, size.Diameter, _nozzlePppointCounter++, element.Coco);
            //TODO: handle p3 for direction
            if (_nozzlePppointCounter == 1) {
                var p = new Position();
                p.X = 0;
                p.Y = 0;
                p.Z = 0;
                ConnectingPTCA("Z", p, 0, 3, "");
            }

        }
        public DbElement ConnectingPTCA(string direction, Position position, double size, int number, string coco) {
            var ptca = _ptse.Create(1, DbElementTypeInstance.PTCAR);
            ptca.SetAttribute(DbAttributeInstance.NUMB, number);

            var error = string.Empty;
            RunPMLCommandInParentheses(ptca, "PX", position.XString(), out error);
            RunPMLCommandInParentheses(ptca, "PY", position.YString(), out error);
            RunPMLCommandInParentheses(ptca, "PZ", position.ZString(), out error);

            RunPMLCommand(ptca, "PBOR", size.ToString(), out error);
            RunPMLCommand(ptca, "PCON", coco, out error);
            RunPMLCommand(ptca, "PTCDI", direction, out error);


            return ptca;
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

            int number = _ppointCounter++;
            //ptca.SetAttribute(DbAttributeInstance.NUMB, number);
            RunPMLCommand(ptca, "NUMB", number.ToString(), out error);

            RunPMLCommandInParentheses(ptca, "PX", position.XString(), out error);
            RunPMLCommandInParentheses(ptca, "PY", position.YString(), out error);
            RunPMLCommandInParentheses(ptca, "PZ", position.ZString(), out error);
            RunPMLCommand(ptca, "PTCDI", direction, out error);

            return ptca;
        }
        private bool RunPMLCommandInParentheses(DbElement ce, string attribute, string command, out string error) {
            return RunPMLCommand(ce, attribute, $"({command})", out error);
        }
        private bool RunPMLCommand(DbElement ce, string attribute, string command, out string error) {
            if (command.Length == 0) {
                error = "Command is empty";
                return false;
            }
            CurrentElement.Element = ce;
            Command pmlCommand;

            pmlCommand = Command.CreateCommand(attribute + " " + command);

            var result = pmlCommand.RunInPdms();
            error = pmlCommand.Error.MessageText();
            return result;
        }
    }
}
