using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    public class Convert
    {
        private Dictionary<DbElementType, IBuildableConverter> _converters = new Dictionary<DbElementType, IBuildableConverter>();
        public Convert() {
            //TODO: add reflection
            AddToConverters(new Extr());
            AddToConverters(new Box());
            AddToConverters(new Cone());
            AddToConverters(new Cyli());
            AddToConverters(new Dish());
            AddToConverters(new Nozz());
            AddToConverters(new Ctor());
            AddToConverters(new Pyra());
        }

        private void AddToConverters(IBuildableConverter converter) {
            _converters.Add(converter.SourceType, converter);
        }
        public Buildable ElementToBuildable(DbElement element) {
            if (_converters.ContainsKey(element.ElementType)) {
                return _converters[element.ElementType].Convert(element);
            }
            return null;
        }
    }
}
