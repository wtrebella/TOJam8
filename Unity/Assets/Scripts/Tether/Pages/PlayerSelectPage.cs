using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerSelectPage : TPage
{
	public FLabel allReadyLabel;

	public FLabel infoLabel;

	public FContainer panelHolder;

	public bool areAllPlayersReady = false;

	public FSliceButton startButton;

	public List<PlayerSelectPanel> panels = new List<PlayerSelectPanel>();

	public PlayerSelectPage()
	{

	}
	
	override public void Start()
	{
		FSoundManager.StopMusic();

		FSprite logo = new FSprite("TetherLogo");
		logo.y = 270.0f;
		AddChild(logo);

//		FLabel titleLabel = new FLabel("CubanoBig", "TETHER");
//		AddChild(titleLabel);
//		titleLabel.y = 300;

		allReadyLabel = new FLabel("CubanoBig", "");
		AddChild(allReadyLabel);
		allReadyLabel.y = 190;
		allReadyLabel.scale = 0.75f;

		infoLabel = new FLabel("Franchise", "CLICK THE BOXES OR PRESS BACK/SELECT TO TOGGLE PLAYERS");
		AddChild(infoLabel);
		infoLabel.y = -332;
		infoLabel.scale = 0.5f;
		infoLabel.alpha = 0.0f;

		FLabel smallLabel;

		smallLabel = new FLabel("Franchise", "F: FULLSCREEN");
		AddChild(smallLabel);
		smallLabel.x = -634;
		smallLabel.y = 336.0f;
		smallLabel.alignment = FLabelAlignment.Left;
		smallLabel.scale = 0.4f;
		smallLabel.alpha = 0.7f;

		smallLabel = new FLabel("Franchise", "M: MUTE");
		AddChild(smallLabel);
		smallLabel.x = -634;
		smallLabel.y = 336.0f - 28.0f * 1;
		smallLabel.alignment = FLabelAlignment.Left;
		smallLabel.scale = 0.4f;
		smallLabel.alpha = 0.7f;

		smallLabel = new FLabel("Franchise", "R: RESET");
		AddChild(smallLabel);
		smallLabel.x = -634;
		smallLabel.y = 336.0f - 28.0f * 2;
		smallLabel.alignment = FLabelAlignment.Left;
		smallLabel.scale = 0.4f;
		smallLabel.alpha = 0.7f;

		startButton = new FSliceButton(220, 80, "Popup_Bg", "Popup_Bg", Color.blue, Color.white, "click1");
		AddChild(startButton);
		startButton.x = 425.0f;
		startButton.y = 270;
		startButton.sprite.alpha = 0.3f;
		startButton.AddLabelA("CubanoBig", "START!", 0.75f, 2f, Color.white);
		startButton.SignalRelease += HandleStartButtonClick;

		Go.to(infoLabel, 0.5f, new TweenConfig().floatProp("alpha", 0.7f).setDelay(0.7f));

		float spreadX = 275;
		float spreadY = 115;

		AddChild(panelHolder = new FContainer());
		panelHolder.y = -80.0f;

		List<Player> players = GameManager.instance.players;

		for(int p = 0; p<players.Count; p++)
		{
			Player player = players[p];

			if(player.controller != GameManager.instance.unusedPlayerController && player.controller.CanBeUsed())
			{
				AIPlayerController aiController = player.controller as AIPlayerController;

				if(aiController != null)
				{
					player.controller.SetPlayer(null);
					player.controller = aiController.symbolic;
				}
			}
		}

		CreatePlayerPanel(0, -spreadX, spreadY);
		CreatePlayerPanel(1, spreadX, spreadY);
		CreatePlayerPanel(2, -spreadX, -spreadY);
		CreatePlayerPanel(3, spreadX, -spreadY);

		HandlePanelStateChange();

		ListenForUpdate (HandleUpdate);
	}

	void CreatePlayerPanel(int index, float createX, float createY)
	{
		PlayerSelectPanel panel = new PlayerSelectPanel(GameManager.instance.players [index]);
		panelHolder.AddChild(panel);
		panel.SetPosition(createX, createY);
		panel.signalStateChange = HandlePanelStateChange;

		panel.scale = 0.5f;
		panel.alpha = 0.0f;

		Go.to(panel, 0.5f, new TweenConfig().floatProp("scale", 1.0f).floatProp("alpha", 1.0f).setEaseType(EaseType.BackOut).setDelay((float)index*0.2f));

		panels.Add(panel);
	}

	void HandlePanelStateChange()
	{
		List<Player> players = GameManager.instance.players;

		int readyPlayersCount = 0;

		for(int p = 0; p<players.Count; p++)
		{
			Player player = players[p];

			if(player.controller != GameManager.instance.unusedPlayerController && player.controller.CanBeUsed())
			{
				readyPlayersCount++;
			}

		}

		if (readyPlayersCount >= 2)
		{
			allReadyLabel.text = "PRESS START/ENTER TO BEGIN!";
			allReadyLabel.color = RXColor.GetColorFromHex(0xEECCFF);
			areAllPlayersReady = true;
        }
		else
		{
			allReadyLabel.text = "WAITING FOR PLAYERS...";
			allReadyLabel.color = RXColor.GetColorFromHex(0xFF9933);
			areAllPlayersReady = false;
		}
	}
	
	protected void HandleUpdate ()
	{
		if(startButton.isEnabled)
		{
			startButton.scale = 1.0f + Mathf.Sin(Time.time * 10.0f) * 0.025f;
		}
		else
		{
			startButton.scale = 1.0f;
		}

		List<Player> players = GameManager.instance.players;


		List<PlayerController> pcs = GameManager.instance.availablePlayerControllers;
		
		for (int c = 0; c<pcs.Count; c++)
		{
			PlayerController pc = pcs[c];

			if(pc.didJustConnect == true)
			{
				FSoundManager.PlaySound("pickUpOrb",0.25f);
				pc.didJustConnect = false;

				for(int p = 0;p<players.Count;p++)
				{
					Player player = players[p];
					
					if(pc.CanBeUsed() && player.controller == GameManager.instance.unusedPlayerController)
					{
						panels[p].SetController(pc);
						break;
					}
				}
			}

			if(pc.didJustDisconnect == true)
			{
				FSoundManager.PlaySound("orbAppears", 0.25f);
				pc.didJustDisconnect = false;
			}
		}

		if(areAllPlayersReady)
		{
			if(GameConfig.SHOULD_SKIP_CHAR_SELECT)
			{
				StartGame();
				return;
			}

			for(int p = 0;p<players.Count;p++)
			{
				Player player = players[p];
			
				if(player.controller.CanBeUsed())
				{
					if(player.controller.GetButtonDown(PlayerControllerButtonType.Start))
					{
						StartGame();
						return;
					}
				}
			}

			if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
			{
				StartGame();
				return;
			}

			startButton.alpha = 1.0f;
			startButton.isEnabled = true;
		}
		else
		{
			startButton.alpha = 0.0f; //hidden when disabled
			startButton.isEnabled = false;
		}
    }

	void HandleStartButtonClick (FSliceButton button)
	{
		if(areAllPlayersReady)
		{
			StartGame();
		}
		else
		{

		}
	}

	void StartGame()
	{
		List<Player> activePlayers = new List<Player>();

		List<Player> players = GameManager.instance.players;
		
		for (int p = 0; p<players.Count; p++)
		{
			Player player = players [p];
			
			if(player.controller != GameManager.instance.unusedPlayerController && player.controller.CanBeUsed())
			{
				activePlayers.Add(player);

				AISymbolicPlayerController aiController = player.controller as AISymbolicPlayerController;

				if(aiController != null)
				{
					player.controller = aiController.CreateActualController();
					player.controller.SetPlayer(player);
				}
			}
		}

		GameManager.instance.SetRoundData(activePlayers);
		TMain.instance.GoToPage(TPageType.PageInGame);
	}
}