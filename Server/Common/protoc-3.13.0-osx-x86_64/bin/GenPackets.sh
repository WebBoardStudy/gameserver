protoc --version
protoc -I=./ --csharp_out=./ ./Protocol.proto

../../../../Server/PacketGenerator/bin/PacketGenerator ./Protocol.proto
cp -f Protocol.cs "../../../../Client/Assets/Scripts/Packet"
cp -f Protocol.cs "../../../../Server/Server/Packet"
cp -f ClientPacketManager.cs "../../../../Client/Assets/Scripts/Packet"
cp -f ServerPacketManager.cs "../../../../Server/Server/Packet"
