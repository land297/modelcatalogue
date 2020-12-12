using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class Geometry {
        private int _ppointCounter = 8;

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

            var lpyr = new Lpyr(PTCA,_gmse);
            var sext = new Sext(PTCA, _gmse);
            //TODO: add reflection
            _createMapping.Add(DbElementTypeInstance.SEXTRUSION, sext.SEXT);
            _createMapping.Add(DbElementTypeInstance.LPYRAMID, lpyr.LPYR);
            _createMapping.Add(DbElementTypeInstance.SDSH, SDSH);
            _createMapping.Add(DbElementTypeInstance.SCYLINDER, SCYL);
            _createMapping.Add(DbElementTypeInstance.LSNOUT, LSNO);
            _createMapping.Add(DbElementTypeInstance.NOZZLE, PPointForNozzle);
            _createMapping.Add(DbElementTypeInstance.SCTORUS, SCTO);


        }

        public void AddGeometry(IEnumerable<Buildable> builables) {
            foreach (Buildable e in builables.Where(b => b != null)) {
                if (_createMapping.ContainsKey(e.BuildAs)) {
                    _createMapping[e.BuildAs].Invoke(e);
                }
            }
        }


        
        public void SDSH(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SDSH);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);

            var error = string.Empty;
            PMLCommander.RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PHEI", size.Height.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PRAD", size.ZLength.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PAXI", $"P{ptca.GetAsString(DbAttributeInstance.NUMB)}", out error);

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
            PMLCommander.RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PAAX", $"P{ptca1.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(geom, "PBAX", $"P{ptca2.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        public void SCYL(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptca = PTCA(direction.X, position);
            var geom = _gmse.Create(1, DbElementTypeInstance.SCYLINDER);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            PMLCommander.RunPMLCommand(geom, "PDIA", size.Diameter.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PHEI", size.Height.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PAXIS", $"-P{ptca.GetAsString(DbAttributeInstance.NUMB)}", out error);
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

            PMLCommander.RunPMLCommand(geom, "PTDI", size.Height.ToString(), out var error);
            //invert top and bot in paragon...
            PMLCommander.RunPMLCommand(geom, "PTDM", size.BotDiameter.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "PBDM", size.TopDiameter.ToString(), out error);
            PMLCommander.RunPMLCommand(geom, "POFF", "0", out error);


            PMLCommander.RunPMLCommand(geom, "PAAXIS", $"-P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(geom, "PBAXIS", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
        private int _nozzlePpointIndex = 0;
        private int[] _validNozzlePpoints = new int[6] { 1, 2, 4, 5, 6, 7};
        private List<int> _usedNozzlePpoints = new List<int>();
        public void PPointForNozzle(Buildable element) {
        
            //TODO: handle p3 for direction
            if (!_usedNozzlePpoints.Contains(3)) {
                var p = new Position();
                p.X = 0;
                p.Y = 0;
                p.Z = 0;
                ConnectingPTCA("Z", p, 0, 3, "");
                _usedNozzlePpoints.Add(3);
            }

            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;

            int ppoint = -1;
            if (element.NozzleConfig.Ppoint == -1) {
                if (_nozzlePpointIndex < _validNozzlePpoints.Length) {
                    ppoint = _validNozzlePpoints[_nozzlePpointIndex++];
                }
            } else {
                ppoint = element.NozzleConfig.Ppoint;
            }
            if (_usedNozzlePpoints.Contains(ppoint) || ppoint <= 0 || ppoint > _validNozzlePpoints.Last()) {

                PMLCommander.RunPMLCommand($"Invalid ppoint {ppoint} supplied for nozzle, will not create");
                return;

            } else {
                _usedNozzlePpoints.Add(ppoint);
            }

            ConnectingPTCA(direction.Z, position, size.Diameter, ppoint , element.NozzleConfig.Coco);
        }
        public DbElement ConnectingPTCA(string direction, Position position, double size, int number, string coco) {
            var ptca = _ptse.Create(1, DbElementTypeInstance.PTCAR);
            ptca.SetAttribute(DbAttributeInstance.NUMB, number);

            var error = string.Empty;
            PMLCommander.RunPMLCommandInParentheses(ptca, "PX", position.XString(), out error);
            PMLCommander.RunPMLCommandInParentheses(ptca, "PY", position.YString(), out error);
            PMLCommander.RunPMLCommandInParentheses(ptca, "PZ", position.ZString(), out error);

            PMLCommander.RunPMLCommand(ptca, "PBOR", size.ToString(), out error);
            PMLCommander.RunPMLCommand(ptca, "PCON", coco, out error);
            PMLCommander.RunPMLCommand(ptca, "PTCDI", direction, out error);


            return ptca;
        }
        public DbElement PTCA(string direction, Position position) {
            var error = string.Empty;
            
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
            PMLCommander.RunPMLCommand(ptca, "NUMB", number.ToString(), out error);

            PMLCommander.RunPMLCommandInParentheses(ptca, "PX", position.XString(), out error);
            PMLCommander.RunPMLCommandInParentheses(ptca, "PY", position.YString(), out error);
            PMLCommander.RunPMLCommandInParentheses(ptca, "PZ", position.ZString(), out error);
            PMLCommander.RunPMLCommand(ptca, "PTCDI", direction, out error);

            return ptca;
        }
       
    }
}
