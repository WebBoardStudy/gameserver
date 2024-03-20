using Google.Protobuf;
using Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        if (movePacket == null)
            return;
        ClientSession clientSession = session as ClientSession;
        Console.WriteLine($"C_Move [{clientSession.MyPlayer.Info.ObjectId}] : ({movePacket.PosInfo.PosX} , {movePacket.PosInfo.PosY})");

        var player = clientSession.MyPlayer;
        if (player == null)
            return;
        var room = player.Room;
        if (room == null)
            return;

        room.HandleMove(player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        var skillPacket = packet as C_Skill;
        if (skillPacket == null)
            return;
        ClientSession clientSession = session as ClientSession;
        var player = clientSession.MyPlayer;
        if (player == null)
            return;
        var room = player.Room;
        if (room == null)
            return;

        room.HandleSkill(player, skillPacket);
    }
}
