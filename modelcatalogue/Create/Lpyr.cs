using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create {
    public class Lpyr {
        Func<string, Position, DbElement> _ptca;
        public DbElement _gmse;
        public Lpyr(Func<string, Position, DbElement> ptca, DbElement gmse) {
            _ptca = ptca;
            _gmse = gmse;
        }

        public void LPYR(Buildable element) {
            Direction direction = element.Direction;
            Position position = element.Position;
            Size size = element.Size;
            var ptcaX = _ptca(direction.X, position);
            var ptcaY = _ptca(direction.Y, position);
            var ptcaZ = _ptca(direction.Z, position);
            var lpyr = _gmse.Create(1, DbElementTypeInstance.LPYRAMID);

            lpyr.SetAttribute(DbAttributeInstance.TUFL, true);
            var error = string.Empty;
            if (element.SourceType == DbElementTypeInstance.PYRAMID) {
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PBTP", size.XTop.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PCTP", size.YTop.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PBBT", size.XBottom.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PCBT", size.YBottom.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PTDI", size.Height.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PBOF", size.XOffset.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PCOF", size.YOffset.ToString("0.##"), out error);
            } else {
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PBTP", size.XLength.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PCTP", size.YLength.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PBBT", size.XLength.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PCBT", size.YLength.ToString("0.##"), out error);
                PMLCommander.RunPMLCommandInParentheses(lpyr, "PTDI", size.ZLength.ToString("0.##"), out error);
            }


            PMLCommander.RunPMLCommand(lpyr, "PBAX", $"P{ptcaX.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(lpyr, "PCAX", $"P{ptcaY.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(lpyr, "PAAX", $"P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
            PMLCommander.RunPMLCommand(lpyr, "PAAX", $"P{ptcaZ.GetAsString(DbAttributeInstance.NUMB)}", out error);
        }
    }
}
