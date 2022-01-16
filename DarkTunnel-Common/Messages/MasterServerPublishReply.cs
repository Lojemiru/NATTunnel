using System.IO;

namespace DarkTunnel.Common.Messages
{
    [MessageTypeAttribute(MessageType.MASTERSERVERPUBLISHREPLY)]
    public class MasterServerPublishReply : IMessage
    {
        public int id;
        public bool status;
        public string message;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(status);
            writer.Write(message);
        }
        public void Deserialize(BinaryReader reader)
        {
            id = reader.ReadInt32();
            status = reader.ReadBoolean();
            message = reader.ReadString();
        }
    }
}
