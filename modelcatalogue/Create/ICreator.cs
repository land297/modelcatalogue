using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create {
    interface ICreator {
        bool Create(Buildable element);
    }
}
