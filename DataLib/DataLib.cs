using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;


namespace DataLib
{
    [Serializable]
    public class Packet
    {

        public List<string> Gdata;
        public int packetInt;
        public string SenderID;
        public bool packetbool;
        public PacketType PacketType;

        public Packet(PacketType type, string SenderID)
        {

            Gdata = new List<string>();
            this.SenderID = SenderID;
            this.PacketType = type;
        }


        public Packet(byte[] packetBytes)
        {
            BinaryFormatter br = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);
            Packet p = (Packet)br.Deserialize(ms);
            ms.Close();
            this.Gdata = p.Gdata;
            this.packetbool = p.packetbool;
            this.SenderID = p.SenderID;
            this.PacketType = p.PacketType;
        }

        public byte[] ToBytes()
        {
            BinaryFormatter br = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            br.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            ms.Close();
            return bytes;
        }
    }

    public enum PacketType
    {
        name,
        mMessage
    }
}
