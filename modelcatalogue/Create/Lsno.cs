using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create {
    public class Lsno {
        Func<string, Position, DbElement> _ptca;
        public DbElement _gmse;
        public Lsno(Func<string, Position, DbElement> ptca, DbElement gmse) {
            _ptca = ptca;
            _gmse = gmse;
        }

        public void LSNO(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptcaY = _ptca(direction.Y, position);
            var ptcaZ = _ptca(direction.Z, position);
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
    }
}
