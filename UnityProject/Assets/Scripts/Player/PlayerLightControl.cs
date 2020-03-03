﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Mirror;

[Serializable]
public class PlayerLightData
{
	public float Intensity = 0.0f;
	public Color Colour;
	//todo Make it so badmins can Mess around with the sprite so It can be set to anything they desire
	//public Sprite Sprite;
	public EnumSpriteLightData EnumSprite;
	public float Size = 12;
	public ActionData ActionData;
}

public enum EnumSpriteLightData
{
	Default,
	Square,
	Clown,
}

[RequireComponent(typeof(Pickupable))]
public class PlayerLightControl : NetworkBehaviour, IServerInventoryMove, IClientInventoryMove, IActionGUI
{
	public LightEmissionPlayer LightEmission;

	public HashSet<NamedSlot> CompatibleSlots = new HashSet<NamedSlot>() {
		NamedSlot.leftHand,
		NamedSlot.rightHand,
		NamedSlot.suitStorage,
		NamedSlot.belt,
		NamedSlot.back,
		NamedSlot.storage01,
		NamedSlot.storage02,
		NamedSlot.suitStorage,
		NamedSlot.head
	};

	public float Intensity;
	public Color Colour;
	//public Sprite Sprite;
	public EnumSpriteLightData EnumSprite;
	public float Size;
	[SerializeField]
	private ActionData actionData;
	public ActionData ActionData => actionData;
	public PlayerLightData PlayerLightData;

	void Start()
	{
		PlayerLightData = new PlayerLightData()
		{
			Intensity = Intensity,
			Colour = Colour,
			EnumSprite = EnumSprite,
			Size = Size,
			ActionData = ActionData,
		};
	}


	public void OnInventoryMoveServer(InventoryMove info)
	{
		//was it transferred from a player's visible inventory?
		if (info.FromPlayer != null && LightEmission != null)
		{
			LightEmission.RemoveLight(PlayerLightData);
			LightEmission = null;
		}

		if (info.ToPlayer != null)
		{
			if (CompatibleSlots.Contains(info.ToSlot.NamedSlot.GetValueOrDefault(NamedSlot.none)))
			{
				LightEmission = info.ToPlayer.GetComponent<LightEmissionPlayer>();
				LightEmission.AddLight(PlayerLightData);
				var pna = PlayerManager.LocalPlayerScript.playerNetworkActions;
				var showAlert = pna.GetActiveHandItem() == gameObject ||
							pna.GetOffHandItem() == gameObject;
				UIActionManager.Toggle(this, showAlert);
			}
		}
	}

	public void OnInventoryMoveClient(ClientInventoryMove info)
	{
		//UIActionManager.Toggle(this, showAlert);
	}

	public void Toggle(bool on, float intensity = -1)
	{
		if (intensity > -1)
		{
			PlayerLightData.Intensity = intensity;
		}

		if (LightEmission == null)
		{
			return;
		}

		if (on)
		{
			if (LightEmission.ContainsLight(PlayerLightData))
			{
				LightEmission.UpdateLight(PlayerLightData);
			}
			else
			{
				LightEmission.AddLight(PlayerLightData);
			}
		}
		else
		{
			LightEmission.RemoveLight(PlayerLightData);
		}
	}

	public void CallActionClient()
	{
		bool NewLightState = false;
		if(LightEmission = null)
		{
			NewLightState = true;
		}
		Toggle(NewLightState, Intensity);
	}
}

