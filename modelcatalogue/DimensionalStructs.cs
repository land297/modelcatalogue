using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue {

    public struct Size {
        public double XLength, YLength, ZLength, Diameter, Height, TopDiameter, BotDiameter;
        internal double XBottom;
        internal double YBottom;
        internal double XTop;
        internal double YTop;
        internal double XOffset;
        internal double YOffset;
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
