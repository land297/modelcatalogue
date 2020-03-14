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
        public static bool ValidSkey(DbElement equipment) {
            var skey = string.Empty;
            foreach (DbElement text in new DBElementCollection(equipment, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Name().Split('/').Last() == "SKEY") {
                    skey = text.GetString(DbAttributeInstance.STEX);
                }
            }
            return skey == string.Empty ? false : true;
        }
        public static bool ValidDtxr(DbElement equipment) {
            var dtxr = string.Empty;
            foreach (DbElement text in new DBElementCollection(equipment, new TypeFilter(DbElementTypeInstance.TEXT))) {
                if (text.Name().Split('/').Last() == "DETAILTEXT") {
                    dtxr = text.GetString(DbAttributeInstance.STEX);
                }
            }
            return dtxr == string.Empty ? false : true;
        }
        private DbElement SDTE(DbElement cate, string detailText, string skey) {
            DbElement sdte = cate.Members().SingleOrDefault(m => m.ElementType == DbElementTypeInstance.SDTEXT);
            if (sdte == null || sdte.IsValid == false) {
                sdte = cate.CreateLast(DbElementTypeInstance.SDTEXT);
            }
            RunPMLCommand(sdte, "SKEY", $"'{skey}'", out var error);
            RunPMLCommand(sdte, "RTEXT", $"'{detailText}'", out error);
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

        private bool RunPMLCommand(DbElement ce, string attribute, string command, out string error) {
            if (command.Length == 0) {
                error = "Command is empty";
                return false;
            }
            CurrentElement.Element = ce;
            Command pmlCommand;

            pmlCommand = Command.CreateCommand(attribute + " " + command);

            var result = pmlCommand.RunInPdms();
            error = pmlCommand.Error.MessageText();
            return result;
        }
    }
}
