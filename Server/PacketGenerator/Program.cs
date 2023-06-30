using System;
using System.IO;
using System.Xml;

namespace PacketGenerator {
    class Program {
        static string genPackets;
        static void Main(string[] args) {
            XmlReaderSettings settings = new XmlReaderSettings() {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings)) {
                r.MoveToContent();
                while (r.Read()) {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) {
                        ParsePacket(r);
                    }
                }

                File.WriteAllText("GenPacket.cs", genPackets);
            }
        }

        private static void ParsePacket(XmlReader r) {
            if (r.NodeType == XmlNodeType.EndElement) {
                return;
            }

            if (r.Name.ToLower() != "packet") {
                Console.WriteLine("invalid packet node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName)) {
                Console.WriteLine("Packet without Name!");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);

        }

        private static Tuple<string, string, string> ParseMembers(XmlReader r) {
            string packetName = r["name"];
            string memberCode = "";
            string readCode = "";
            string writeCode = "";
            int depth = r.Depth + 1;
            while (r.Read()) {
                if (r.Depth != depth) {
                    break;
                }
                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName)) {
                    Console.WriteLine("Member without name");
                    return new Tuple<string, string, string>("", "", "");
                }

                if (string.IsNullOrEmpty(memberCode) == false) {
                    memberCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(readCode) == false) {
                    readCode += Environment.NewLine;
                }
                if (string.IsNullOrEmpty(writeCode) == false) {
                    writeCode += Environment.NewLine;
                }


                string memberType = r.Name.ToLower();
                switch (memberType) {
                    case "bool":
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        var res = ParseList(r);
                        memberCode += res.Item1;
                        readCode += res.Item2;
                        writeCode += res.Item3;
                        break;
                    default:
                        break;

                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r) {
            string listName = r["name"];
            if (string.IsNullOrEmpty(listName)) {
                Console.WriteLine("List without name");
                return new Tuple<string, string, string>("", "", "");
            }

            var res = ParseMembers(r);
            string memberCode = string.Format(PacketFormat.memberListFormat, FirstCharToUpper(listName), FirstCharToLower(listName), res.Item1, res.Item2, res.Item3);
            string readCode = string.Format(PacketFormat.readListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));
            string writeCode = string.Format(PacketFormat.writeListFormat, FirstCharToUpper(listName), FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType) {
            switch (memberType) {
                case "bool": return "ToBoolean";
                case "short": return "ToInt16";
                case "ushort": return "ToUInt16";
                case "int": return "ToInt32";
                case "long": return "ToInt64";
                case "float": return "ToSingle";
                case "double": return "ToDouble";
                default:
                    return "";
            }
        }

        public static string FirstCharToUpper(string value) {
            if (string.IsNullOrEmpty(value)) return "";
            return value[0].ToString().ToUpper() + value.Substring(1);
        }

        public static string FirstCharToLower(string value) {
            if (string.IsNullOrEmpty(value)) return "";
            return value[0].ToString().ToLower() + value.Substring(1);
        }
    }
}

