using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PacketHandler {
    public static void S_ChatHandler(PacketSession session, IPacket packet) {
        S_Chat pk = packet as S_Chat;
        
        //Console.WriteLine($"From:{pk.playerId} {pk.chat} send");
    }
}

