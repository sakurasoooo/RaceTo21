using System;
using System.Numerics;

namespace RaceTo21
{
    /// <summary>
    /// Indicates the player's state
    /// active = player can continue to draw cards,
    /// stay = player chooses to stop drawing cards,
    /// bust = player's hand score exceeds 21,
    /// wi n= player's hand score is equal to 21
    /// </summary>
    public enum PlayerStatus
	{
		active,
		stay,
		bust,
		win
	}
}

