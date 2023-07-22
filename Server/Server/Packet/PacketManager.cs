
using ServerCore;

public class PacketManager {
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance {
        get {            
            return _instance;
        }
    }
    public PacketManager() {
        Register();
    }
    #endregion

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer) {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action;
        if (_onRecv.TryGetValue(packetId, out action)) {
            action.Invoke(session, buffer);
        }       
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new() {
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }

    public void Register() {
      
        _onRecv.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        _handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);

    }
}
