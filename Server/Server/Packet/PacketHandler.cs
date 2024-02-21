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
		Console.WriteLine($"C_Move [{clientSession.MyPlayer.Info.PlayerId}] : ({movePacket.PosInfo.PosX} , {movePacket.PosInfo.PosY})");
		if (clientSession?.MyPlayer.Room == null)
			return;

		PlayerInfo playerInfo = clientSession.MyPlayer.Info;
		playerInfo.PosInfo = movePacket.PosInfo;

		S_Move sMove = new S_Move
		{
			PlayerId = playerInfo.PlayerId,
			PosInfo = movePacket.PosInfo
		};
		clientSession.MyPlayer.Room.Broadcast(sMove);
		
	}
}
