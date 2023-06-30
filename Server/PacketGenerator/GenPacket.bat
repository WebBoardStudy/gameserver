cd ./bin
start PacketGenerator.exe ../PDL.xml
xcopy GenPacket.cs "../../Server/Packet/GenPackets.cs" /Y /F
xcopy GenPacket.cs "../../DummyClient/Packet/GenPackets.cs" /Y /F