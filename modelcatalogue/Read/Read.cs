//using Aveva.Core.Database;
//using Aveva.Core.Database.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace modelcatalogue.ReadModel {
//    static public class Read {

//        static public Buildable ReadElement(DbElement element) {
//            return ReadElement(element, 1);
//        }

//        static public Buildable ReadElement(DbElement element, int number) {
//            Console.WriteLine(element.ElementType.ShortName);
//            switch (element.ElementType.ShortName) {
//                case "DISH":
//                    return TransformToElement(element, DISH(element));
//                case "CYLI":
//                    return TransformToElement(element, CYLI(element));
//                case "BOX":
//                    return TransformToElement(element, BOX(element));
//                case "CONE":
//                    return TransformToElement(element, CONE(element));
//                case "EXTR":
//                    return EXTR(element);
//                case "NOZZ":
//                    return TransformToElement(element, PPoint(element, number));
//            }

//            return new Buildable();
//        }

//        static private Buildable TransformToElement(DbElement dbElement, Dictionary<AttributeInstance, object> attributeValues) {
//            Size size = new Size();
//            Position position = new Position();
//            Direction direction = new Direction();
//            Dictionary<string, string> values = new Dictionary<string, string>();
//            string coco = string.Empty;
//            foreach (KeyValuePair<AttributeInstance,object> kvp in attributeValues) {
//                switch(kvp.Key) {
//                    case AttributeInstance.HEIG:
//                        size.Height = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.DIAM:
//                        size.Diameter = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.PPOS:
//                        position.X = ((Aveva.Core.Geometry.Position)kvp.Value).X;
//                        position.Y = ((Aveva.Core.Geometry.Position)kvp.Value).Y;
//                        position.Z = ((Aveva.Core.Geometry.Position)kvp.Value).Z;
//                        break;
//                    case AttributeInstance.PDIR:
//                        //direction = 1, orientation = several directions...
//                        direction.X = ((Aveva.Core.Geometry.Direction)kvp.Value).ToString();
//                        direction.Y = ((Aveva.Core.Geometry.Direction)kvp.Value).ToString();
//                        direction.Z = ((Aveva.Core.Geometry.Direction)kvp.Value).ToString();
//                        break;
//                    case AttributeInstance.ORIE:
//                        direction.X = ((Aveva.Core.Geometry.Orientation)kvp.Value).XDir().ToString();
//                        direction.Y = ((Aveva.Core.Geometry.Orientation)kvp.Value).YDir().ToString();
//                        direction.Z = ((Aveva.Core.Geometry.Orientation)kvp.Value).ZDir().ToString();
//                        break;
//                    case AttributeInstance.XLEN:
//                        size.XLength = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.YLEN:
//                        size.YLength = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.ZLEN:
//                        size.ZLength = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.DTOP:
//                        size.TopDiameter = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.DBOT:
//                        size.BotDiameter = (double)kvp.Value;
//                        break;
//                    case AttributeInstance.COCO:
//                       coco = (string)kvp.Value;
//                        break;
//                }
//            }

//            var element = new Buildable(dbElement, size, position, direction, coco);
//            return element;
//        }

//        static Dictionary<AttributeInstance,object> PPoint(DbElement element, int ppointNumber) {
//            Dictionary<AttributeInstance, object> values = new Dictionary<AttributeInstance, object>();

//            var q = new DbQualifier();
//            q.Add(ppointNumber);
//            var v1 = element.GetDouble(DbAttributeInstance.PPBO, q);
//            var v2 = element.GetString(DbAttributeInstance.PPCO, q);
//            var v4 = element.GetPosition(DbAttributeInstance.PPOS, q);
//            var v5 = element.GetDirection(DbAttributeInstance.PDIR, q);

//            values.Add(AttributeInstance.DIAM, v1);
//            values.Add(AttributeInstance.COCO, v2);
//            values.Add(AttributeInstance.PPOS, v4);
//            values.Add(AttributeInstance.PDIR, v5);
//            values.Add(AttributeInstance.TYPESHORTNAME, element);

//            return values;

//        }
//        static Dictionary<AttributeInstance, object> DISH(DbElement element) {
//            Dictionary<AttributeInstance, object> values = new Dictionary<AttributeInstance, object>();
//            var v1 = element.GetDouble(DbAttributeInstance.DIAM);
//            var v2 = element.GetDouble(DbAttributeInstance.HEIG);
//            var v3 = element.GetDouble(DbAttributeInstance.RADI);
//            var q = new DbQualifier();
//            q.Add(2);
//            var v4 = element.GetPosition(DbAttributeInstance.PPOS, q);
//            var v5 = element.GetDirection(DbAttributeInstance.PDIR, q);

//            values.Add(AttributeInstance.DIAM, v1);
//            values.Add(AttributeInstance.HEIG, v2);
//            values.Add(AttributeInstance.RADI, v3);
//            values.Add(AttributeInstance.PPOS, v4);
//            values.Add(AttributeInstance.PDIR, v5);
//            values.Add(AttributeInstance.TYPESHORTNAME, element);

//            return values;

//        }
//        static Dictionary<AttributeInstance, object> CYLI(DbElement element) {
//            Dictionary<AttributeInstance, object> values = new Dictionary<AttributeInstance, object>();

//            var v1 = element.GetDouble(DbAttributeInstance.HEIG);
//            var v2 = element.GetDouble(DbAttributeInstance.DIAM);
//            var q = new DbQualifier();
//            q.Add(1);
//            var v3 = element.GetPosition(DbAttributeInstance.PPOS, q);
//            //Orientation Y is N and Z is U
//            var v4 = element.GetDirection(DbAttributeInstance.PDIR, q);
//            var s = v4.ToString();

//            values.Add(AttributeInstance.HEIG, v1);
//            values.Add(AttributeInstance.DIAM, v2);
//            values.Add(AttributeInstance.PPOS, v3);
//            values.Add(AttributeInstance.PDIR, v4);
//            values.Add(AttributeInstance.TYPESHORTNAME, element);

//            return values;
//        }
//        static Buildable EXTR(DbElement element) {
//            Size size = new Size();
//            Position position = new Position();
//            Direction direction = new Direction();

//            size.Height = element.GetDouble(DbAttributeInstance.HEIG);

//            var pos = element.GetPosition(DbAttributeInstance.POS);

//            position.X = pos.X;
//            position.Y = pos.Y;
//            position.Z = pos.Z;

//            var orientation = element.GetOrientation(DbAttributeInstance.ORI);

//            direction.X = orientation.XDir().ToString();
//            direction.Y = orientation.YDir().ToString();
//            direction.Z = orientation.ZDir().ToString();

//            var vertPos = new List<Position>();
//            foreach (var vert in element.FirstMember().Members()) {
//                pos = vert.GetPosition(DbAttributeInstance.POS);
//                var vp = new Position();
//                vp.X = pos.X;
//                vp.Y = pos.Y;
//                vp.Z = pos.Z;

//                vertPos.Add(vp);
//            }
       
//            return new Buildable(element, size, position, direction, vertPos);

//        }

//        static Dictionary<AttributeInstance, object> CONE(DbElement element) {
//            Dictionary<AttributeInstance, object> values = new Dictionary<AttributeInstance, object>();

//            var v1 = element.GetDouble(DbAttributeInstance.DTOP);
//            var v2 = element.GetDouble(DbAttributeInstance.DBOT);
//            var v3 = element.GetDouble(DbAttributeInstance.HEIG);
//            var q = new DbQualifier();
//            q.Add(1);
//            var v4 = element.GetPosition(DbAttributeInstance.PPOS, q);
//            var v5 = element.GetOrientation(DbAttributeInstance.ORI);

//            values.Add(AttributeInstance.DTOP, v1);
//            values.Add(AttributeInstance.DBOT, v2);
//            values.Add(AttributeInstance.HEIG, v3);
//            values.Add(AttributeInstance.PPOS, v4);
//            values.Add(AttributeInstance.ORIE, v5);
//            values.Add(AttributeInstance.TYPESHORTNAME, element);

//            return values;
//        }
//        static Dictionary<AttributeInstance, object> BOX(DbElement element) {
//            Dictionary<AttributeInstance, object> values = new Dictionary<AttributeInstance, object>();

//            var v1 = element.GetDouble(DbAttributeInstance.XLEN);
//            var v2 = element.GetDouble(DbAttributeInstance.YLEN);
//            var v3 = element.GetDouble(DbAttributeInstance.ZLEN);
//            var q = new DbQualifier();
//            q.Add(6);
//            var v4 = element.GetPosition(DbAttributeInstance.PPOS, q);
//            var v5 = element.GetOrientation(DbAttributeInstance.ORI);

//            values.Add(AttributeInstance.XLEN, v1);
//            values.Add(AttributeInstance.YLEN, v2);
//            values.Add(AttributeInstance.ZLEN, v3);
//            values.Add(AttributeInstance.PPOS, v4);
//            values.Add(AttributeInstance.ORIE, v5);
//            values.Add(AttributeInstance.TYPESHORTNAME, element);

//            return values;
//        }
//    }
//}
