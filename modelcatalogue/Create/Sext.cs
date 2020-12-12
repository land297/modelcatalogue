using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create {
    public class Sext {
        Func<string, Position, DbElement> _ptca;
        DbElement _gmse;
        public Sext(Func<string, Position, DbElement> ptca, DbElement gmse) {
            _ptca = ptca;
            _gmse = gmse;
        }
        public void SEXT(Buildable element) {
            var ptcaX = _ptca(element.Direction.X, element.Position);
            var ptcaY = _ptca(element.Direction.Y, element.Position);
            //var ptcaZ = PTCA(element.Direction.Z, element.Position);

            var geom = _gmse.Create(1, DbElementTypeInstance.SEXTRUSION);

            geom.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;

            PMLCommander.RunPMLCommand(geom, "PAAX", $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(geom, "PBAX", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(geom, "PHEI", element.Size.Height.ToString(), out error);

            // no need to set position, as position is set by PAAX and PBAX
            //PMLCommander.RunPMLCommandInParentheses(geom, "PX", element.Position.X.ToString(), out error);
            //PMLCommander.RunPMLCommandInParentheses(geom, "PY", element.Position.Y.ToString(), out error);
            //PMLCommander.RunPMLCommandInParentheses(geom, "PZ", element.Position.Z.ToString(), out error);

            var sloo = geom.Create(1, DbElementTypeInstance.SLOOP);

            foreach (var vert in element.Verticies) {
                var svert = sloo.CreateLast(DbElementTypeInstance.SVERTEX);
                PMLCommander.RunPMLCommandInParentheses(svert, "PX", vert.X.ToString(), out error);
                PMLCommander.RunPMLCommandInParentheses(svert, "PY", vert.Y.ToString(), out error);
            }

        }
    }
}
