cd ./bin
start PacketGenerator.exe ../PDL.xml
xcopy GenPacket.cs "../../Server/Packet/GenPackets.cs" /Y /F
xcopy GenPacket.cs "../../DummyClient/Packet/GenPackets.cs" /Y /F
xcopy ServerPacketManager.cs "../../Server/Packet/PacketManager.cs" /Y /F
xcopy ClientPacketManager.cs "../../DummyClient/Packet/PacketManager.cs" /Y /F