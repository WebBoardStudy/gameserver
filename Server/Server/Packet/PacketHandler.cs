using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"C_MOVE ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

        if (clientSession.MyPlaeyr == null)
            return;
        if (clientSession.MyPlaeyr.Room == null)
            return;
        
        // TODO : 검증
        
        // 일단 서버에서 좌표 이동
        PlayerInfo info = clientSession.MyPlaeyr.info;
        info.PosInfo = movePacket.PosInfo;
        
        // 다른 플레이어한테도 알려준다.
        S_Move resMovePacket = new S_Move();
        resMovePacket.PlayerId = clientSession.MyPlaeyr.info.PlayerId;
        resMovePacket.PosInfo = movePacket.PosInfo;

        clientSession.MyPlaeyr.Room.Broadcast(resMovePacket);
    }
}