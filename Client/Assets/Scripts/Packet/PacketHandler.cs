using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("S_EnterGameHandler");
		Debug.Log(enterGamePacket.Player);
	}
	
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_LeaveGame;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("S_LeaveGameHandler");
	}
	
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Spawn;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("S_SpawnHandler");
		Debug.Log(gamePacket.Players);
	}
	
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Despawn;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("S_DespawnHandler");
	}
	
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;
		
		Debug.Log("S_MoveHandler");
	}
}
