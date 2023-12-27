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
		if (enterGamePacket == null)
		{
			Debug.Log("enterGamePacket is null");
			return;
		}
		
		Managers.Object.Add(enterGamePacket.Player, true);
	}
	
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_LeaveGame;
		if (gamePacket == null)
		{
			Debug.Log("S_LeaveGame is null");
			return;
		}
		Managers.Object.RemoveMyPlayer();
	}
	
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Spawn;
		if (gamePacket == null)
		{
			Debug.Log("S_Spawn is null");
			return;
		}
		
		foreach (var player in gamePacket.Players)
		{
			Managers.Object.Add(player, false);
		}
	}
	
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Despawn;
		if (gamePacket == null)
		{
			Debug.Log("S_Despawn is null");
			return;
		}
		
		foreach (var playerId in gamePacket.PlayerIds)
		{
			Managers.Object.Remove(playerId);
		}
	}
	
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		var gamePacket = packet as S_Move;
		if (gamePacket == null)
		{
			Debug.Log("S_Move is null");
			return;
		}
	}
}
