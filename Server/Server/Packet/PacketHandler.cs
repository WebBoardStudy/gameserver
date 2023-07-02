using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PacketHandler {
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet) {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        Console.WriteLine($"C_PlayerInfoReq: {p.name} {p.playerId}");
        p.skills.ForEach(skill => {
            Console.WriteLine($"\t\t{skill.id} {skill.level} {skill.duration}");
        });
    }
}

