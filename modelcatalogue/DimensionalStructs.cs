using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue {
    class UtilityStructures {
    }

    //should make then inmutable
    public struct Size {
        public double XLength, YLength, ZLength, Diameter, Height, TopDiameter, BotDiameter;
    }

    public struct Position {
        public double X, Y, Z;
        public string XString() {
            return string.Format("{0:0.00}", X);
        }
        public string YString() {
            return string.Format("{0:0.00}", Y);
        }
        public string ZString() {
            return string.Format("{0:0.00}", Z);
        }
    }

    public struct Direction {
        public string X, Y, Z;
        public string XString() {
            return string.Format("{0:0.00}", X);
        }
        public string YString() {
            return string.Format("{0:0.00}", Y);
        }
        public string ZString() {
            return string.Format("{0:0.00}", Z);
        }
    }
}
