using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace modelcatalogue.Create
{
    public class Cata
    {
        public static DbElement GetWritable(DbElement equi) {
            var cataName = equi.Owner.Owner.Name() + "-CATA";
            
            DbElement cata = MDB.CurrentMDB.FindElement(DbType.Catalog, cataName);
            Db db = default(Db);
            if (!cata.IsValid) {
                Db[] dbs = MDB.CurrentMDB.GetDBArray();
                //find cata dbs with dbwrite eq true
             
                db = dbs.FirstOrDefault(d => d.Type == DbType.Catalog && !d.IsReadOnly);

                Console.WriteLine($"can write to:{ db.Number} - { db.IsReadOnly}- { db.Type}");
                cata = db.World.Create(1, DbElementTypeInstance.CATALOGUE);
                cata.SetAttribute(DbAttributeInstance.NAME, cataName);
            } else {
                db = cata.Db;
            }

            return cata;
        }
    }
}
