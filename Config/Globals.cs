using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Auto_Delete_GoldKingZ;

public class Globals
{
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public DateTime EventPlayerChat { get; set; }
        public PlayerDataClass(CCSPlayerController Playerr, DateTime EventPlayerChatt)
        {
            Player = Playerr;
            EventPlayerChat = EventPlayerChatt;
        }
    }
    public Dictionary<int, PlayerDataClass> Player_Data = new Dictionary<int, PlayerDataClass>();


    public void Clear()
    {
        Player_Data?.Clear();
    }
}