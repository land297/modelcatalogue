using Aveva.Core.Database;
using Aveva.Core.Database.Filters;
using Aveva.Core.Utilities.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class Text
    {
        private DbElement SDTE(DbElement cate, string detailText, string skey) {
            DbElement sdte = cate.Members().FirstOrDefault(m => m.ElementType == DbElementTypeInstance.SDTEXT);
            if (sdte == null || sdte.IsValid == false) {
                sdte = cate.CreateLast(DbElementTypeInstance.SDTEXT);
            }
            detailText = detailText.Replace("@SIZE", "' + STRING (PARA[1] ) + '");
            PMLCommander.RunPMLCommand(sdte, "SKEY", $"'{skey}'", out var error);
            PMLCommander.RunPMLCommandInParentheses(sdte, "RTEXT", $"'{detailText}'", out error);
            return sdte;
        }

        public DbElement CreateOrUpdateSDTE(DbElement equipment, DbElement cate) {
            var skey = string.Empty;
            var dtxr = string.Empty;

            foreach (DbElement text in new DBElementCollection(equipment.Owner, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Purpose() == "DTXT") {
                    dtxr = text.GetString(DbAttributeInstance.STEX);
                }
                if (text.Purpose() == "SKEY") {
                    skey = text.GetString(DbAttributeInstance.STEX);
                }
            }

            return SDTE(cate, dtxr, skey);
        }

        
    }
}
