using Google.Protobuf;
using Protocol;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (var obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.PlayerIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        var movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        var skillPacket = packet as S_Skill;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
            return;

        var pc = go.GetComponent<PlayerController>();
        if (pc == null)
            return;
        pc.UseSkill(skillPacket.Info.SkillId);
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        var changeHpPacket = packet as S_ChangeHp;

        var go = Managers.Object.FindById(changeHpPacket.ObjectId);
        if (go == null) return;

        var cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = changeHpPacket.Hp;
        }

    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        var diePk = packet as S_Die;

        var go = Managers.Object.FindById(diePk.ObjectId);
        if (go == null) return;

        var cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = 0;
            cc.OnDead();
        }

    }
}


