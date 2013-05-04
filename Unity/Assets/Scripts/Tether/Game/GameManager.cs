using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager
{
	static public GameManager instance;

	public static void Init() {instance = new GameManager();}

	public List<Player> players = new List<Player>();
	public List<Player> activePlayers = null;
	
	public GameManager()
	{
		players.Add(new Player(0, RXColor.GetColorFromHex(0xFF0000)));
		players.Add(new Player(1, RXColor.GetColorFromHex(0x00FF00)));
		players.Add(new Player(2, RXColor.GetColorFromHex(0x0000FF)));
		players.Add(new Player(3, RXColor.GetColorFromHex(0xFFFF00)));
	}

	public void SetRoundData(List<Player> activePlayers)
	{
		this.activePlayers = activePlayers;
	}
}

