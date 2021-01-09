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
        public DbElement Sdte { get; private set; }
        private DbElement SDTE(DbElement cate, string detailText, string skey, DbElement equipment) {
            var equiDesc = equipment.GetAsString(DbAttributeInstance.DESC);
            DbElement sdte = DbElement.GetElement();

            if (equiDesc == "unset") {
                sdte = cate.Members().FirstOrDefault(m => m.ElementType == DbElementTypeInstance.SDTEXT);
                if (sdte == null || sdte.IsValid == false) {
                    sdte = cate.CreateLast(DbElementTypeInstance.SDTEXT);
                }
            } else {
                var sdteName = equipment.Name() + ".SDTE";
                sdte = cate.Members().FirstOrDefault(m => m.ElementType == DbElementTypeInstance.SDTEXT && m.Name() == sdteName);
                if (sdte == null || sdte.IsValid == false) {
                    sdte = cate.CreateLast(DbElementTypeInstance.SDTEXT);
                    try { 
                        sdte.SetAttribute(DbAttributeInstance.NAME, sdteName);
                        Console.WriteLine("name error for sdte");
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            detailText = detailText.Replace("@SIZE", "' + STRING (PARA[1] ) + '");
            detailText = detailText.Replace("@DESC", equiDesc);
            PMLCommander.RunPMLCommand(sdte, "SKEY", $"'{skey}'", out var error);
            PMLCommander.RunPMLCommandInParentheses(sdte, "RTEXT", $"'{detailText}'", out error);
            Sdte = sdte;
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

            return SDTE(cate, dtxr, skey, equipment);
        }

        public DbElement MatxtElement(DbElement equipment) {
            var possbileMatxts = new DBElementCollection(equipment.Owner,
            new AndFilter(new TypeFilter(DbElementTypeInstance.TEXT),
                          new AttributeStringFilter(DbAttributeInstance.PURP, FilterOperator.Equals, "MTXT")));

            return possbileMatxts.First();
        }

        public string Stype(DbElement equipment) {
            var possbileStypes = new DBElementCollection(equipment.Owner,
           new AndFilter(new TypeFilter(DbElementTypeInstance.TEXT),
             new AttributeStringFilter(DbAttributeInstance.PURP, FilterOperator.Equals, "STYP")));

            var text = possbileStypes.First();
            if (text.IsValid) {
                return text.GetAsString(DbAttributeInstance.STEX);
            } else {
                return "AAAA";
            }
        }
        public string TypeTans(DbElement equipment) {
            var possbileTypes = new DBElementCollection(equipment.Owner,
                          new AndFilter(new TypeFilter(DbElementTypeInstance.TEXT),
                            new AttributeStringFilter(DbAttributeInstance.PURP, FilterOperator.Equals, "TYPE")));

            var text = possbileTypes.First();
            if (text.IsValid) {
                return text.GetAsString(DbAttributeInstance.STEX);
            } else {
                return "VALV";
            }
        }

    }
}
