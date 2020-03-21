using Aveva.Core.Database;
using modelcatalogue.ConvertToBuildable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue {
    public class Buildable {
        public DbElementType BuildAs { get; private set; }
        private DbElement _element;
        public Size Size { get; private set; }
        // TOOD: change to lists..
        public Position Position { get; private set; }
        public Position Position2 { get; private set; }
        public Direction Direction { get; private set; }
        public Direction Direction2 { get; private set; }
        public List<Position> Verticies { get; private set; }
        public NozzleConfig NozzleConfig { get; private set; }
        public string ElementType { get { return _element.ElementType.ShortName; } }
        public Buildable(DbElementType buildAs, DbElement element, Size size, Position position, Direction direction, ConvertToBuildable.NozzleConfig nozzleConfig) {
            _element = element;
            Size = size;
            Position = position;
            Direction = direction;
            NozzleConfig = nozzleConfig;
            BuildAs = buildAs;
        }
        public Buildable(DbElementType buildAs, DbElement element, Size size, Position position, Direction direction) {
            BuildAs = buildAs;
            _element = element;
            Size = size;
            Position = position;
            Direction = direction;
        }
        public Buildable(DbElementType buildAs, DbElement element, Size size, Position position, Direction direction, Position p2, Direction d2) {
            BuildAs = buildAs;
            _element = element;
            Size = size;
            Position = position;
            Direction = direction;
            Position2 = p2;
            Direction2 = d2;
        }
        public Buildable(DbElementType buildAs,DbElement element, Size size, Position position, Direction direction, List<Position> vertices) {
            BuildAs = buildAs;
            _element = element;
            Size = size;
            Position = position;
            Direction = direction;
            Verticies = vertices;
        }

        public Buildable() {

        }
    }
}
