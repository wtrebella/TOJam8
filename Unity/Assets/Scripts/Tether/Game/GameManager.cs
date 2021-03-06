using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager
{
	static public GameManager instance;

	public static void Init() {instance = new GameManager();}

	public List<Player> players = new List<Player>();
	public List<Player> activePlayers = null;

	public List<PlayerController> availablePlayerControllers = new List<PlayerController>();

	public UnusedPlayerController unusedPlayerController;
	public AISymbolicPlayerController aiPlayerController;
	
	public GameManager()
	{
		unusedPlayerController = new UnusedPlayerController();
		aiPlayerController = new AISymbolicPlayerController();
		
		availablePlayerControllers.Add(unusedPlayerController);
		availablePlayerControllers.Add(new KeyboardPlayerController(false));
		availablePlayerControllers.Add(new KeyboardPlayerController(true));
		availablePlayerControllers.Add(new GamepadPlayerController(0));
		availablePlayerControllers.Add(new GamepadPlayerController(1));
		availablePlayerControllers.Add(new GamepadPlayerController(2));
		availablePlayerControllers.Add(new GamepadPlayerController(3));
		availablePlayerControllers.Add(aiPlayerController);

		players.Add(new Player(0, "PURPLE PLAYER", RXColor.GetColorFromHex(0xFF00EE), unusedPlayerController));
		players.Add(new Player(1, "GREEN PLAYER", RXColor.GetColorFromHex(0x00FF00), unusedPlayerController));
		players.Add(new Player(2, "BLUE PLAYER", RXColor.GetColorFromHex(0x0011EE), unusedPlayerController));
		players.Add(new Player(3, "RED PLAYER", RXColor.GetColorFromHex(0xFF0011), unusedPlayerController));
	}

	public void SetRoundData(List<Player> activePlayers)
	{
		this.activePlayers = activePlayers;
	}

	public void Reset()
	{
		foreach (Player player in players)
		{
			player.Reset();
		}
	}

	public PlayerController GetNextAvailablePlayerController(PlayerController currentPC)
	{
		int pcCount = availablePlayerControllers.Count;

		int index = (1 + availablePlayerControllers.IndexOf(currentPC)) % pcCount;

		for(int c = 0; c<pcCount; c++)
		{
			PlayerController pcToCheck = availablePlayerControllers[index];

			if(pcToCheck.CanBeUsed() && pcToCheck.GetPlayer() == null)
			{
				return pcToCheck;
			}

			index = (index+1) % pcCount; //increment the index
		}

		return unusedPlayerController;
	}
}


