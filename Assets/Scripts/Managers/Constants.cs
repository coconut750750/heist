using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Constants {

	/// TAGS
	public const string PLAYER_TAG = "Player";
	public const string NPC_TAG = "NPC";

	public const string STAIRS_TAG = "Stairs";

	public const string BUTTON_B_TAG = "ButtonB";
    public const string BUTTON_A_TAG = "ButtonA";

	public const string NPC_NAME = "NPC-";

	/// LAYERS
	public const int NPC_ONLY_LAYER = 1024;
	public const int PLAYER_ONLY_LAYER = 2048;


	/// SPRITE LAYERS
	public const string ELEVATED1 = "Elevated1";
	public const string ELEVATED2 = "Elevated2";

	/// INPUT AXIS
	public const string HORIZONTAL = "Horizontal";
	public const string VERTICAL = "Vertical";

	/// QUESTS
	public const string SELLING_QUEST = "selling_quest";
}
