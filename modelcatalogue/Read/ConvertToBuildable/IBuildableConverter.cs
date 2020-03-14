using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.ConvertToBuildable
{
    interface IBuildableConverter
    {
        DbElementType SourceType { get; }

        Buildable Convert(DbElement element);
    }
}
