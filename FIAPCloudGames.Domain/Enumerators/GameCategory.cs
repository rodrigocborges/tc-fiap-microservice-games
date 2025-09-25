
using System.ComponentModel;

namespace FIAPCloudGames.Domain.Enumerators
{
    public enum GameCategory
    {
        [Description("Ação")]
        Action,
        [Description("Aventura")]
        Adventure,
        [Description("Terror")]
        Horror,
        [Description("Puzzle")]
        Puzzle,
        [Description("Simulação")]
        Simulation,
        [Description("Estratégia")]
        Strategy,
        [Description("RPG")]
        RPG,
        [Description("Indie")]
        Indie,
        [Description("Educacionais")]
        Educational,
        [Description("Tiro em primeira pessoa")]
        FPS,
        [Description("Tiro em terceira pessoa")]
        TPS
    }
}
