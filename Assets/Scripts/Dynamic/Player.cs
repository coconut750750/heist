using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;
using UnityEngine.Events;

public class Player : Character
{
    // TODO: make this variable based on attack speed
    private const float ATTACK_DELAY_SECONDS = 0.5f;

    public static Vector3 START_POS = new Vector3(0.5f, 0, 0);

    [SerializeField]
    private Pocket pocket;
    private UnityAction currentItemAction;

    [SerializeField]
    private Text moneyText;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Text expText;

    public int suspicion = 0; // 0 (nothing wrong), 1 (chase after), 2 (beat down), 3 (all police suspicious unless signal jammed)

    private bool canAttack = true;
    IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(ATTACK_DELAY_SECONDS);
        canAttack = true;
    }

    protected override void Start()
    {
        base.Start();
        pocket = GetComponent(typeof(Pocket)) as Pocket;
        pocket.SetSelectedConsumeCallback(OnSelectedConsumable);
        pocket.SetDeselectedCallback(delegate
        {
            if (currentItemAction != null)
            {
                Interactable.buttonA.RemoveAction(currentItemAction);
            }
        });

        UpdateUIInfo();
        UpdateVisibleFloorWithGameManager();
    }

    protected override void FixedUpdate()
    {
        float moveHorizontal = 0;
        float moveVertical = 0;

        Vector3 movement;
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        moveHorizontal = Input.GetAxis(Constants.HORIZONTAL);
        moveVertical = Input.GetAxis(Constants.VERTICAL);
        movement = new Vector3(moveHorizontal, moveVertical, 0f);

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis(Constants.HORIZONTAL);
		moveVertical = CrossPlatformInputManager.GetAxis(Constants.VERTICAL);
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

#endif

        Move(movement.normalized);
    }

    /// INTERACTABLES ///
	public float GetAttackDelay() {
		return ATTACK_DELAY_SECONDS;
	}

    public bool Attack()
    {
        if (!canAttack)
        {
            return false;
        }

        base.Attack(Constants.NPC_ONLY_LAYER);
        EventManager.instance.OnPunch();
        StartCoroutine(AttackDelay());

		return true;
    }

    public override void GetAttackedBy(Character other)
    {
        base.GetAttackedBy(other);
        UpdateUIInfo();
    }

    private void OnSelectedConsumable(Item item, int index)
    {
        currentItemAction = delegate
        {
            Consume(item, index);
        };
        Interactable.buttonA.AddAction(currentItemAction);
    }

    public void Consume(Item item, int index)
    {
        RemoveItemAtIndex(index);
        Interactable.buttonA.RemoveAction(currentItemAction);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void OnEnterStairs()
    {
        UpdateVisibleFloorWithGameManager();
    }

    private void UpdateVisibleFloorWithGameManager()
    {
        if (GetFloor() == 1)
        {
            GameManager.instance.HideFloor2();
        }
        else if (GetFloor() == 2)
        {
            GameManager.instance.ShowFloor2();
        }
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public Pocket GetPocket()
    {
        return pocket;
    }

    public void AddItem(Item item)
    {
        pocket.AddItem(item);
    }

    public Item RemoveItemAtIndex(int index)
    {
        if (index >= 0 || index < pocket.GetNumItems())
        {
            Item itemToRemove = pocket.GetItem(index);
            pocket.RemoveItem(itemToRemove);
            return itemToRemove;
        }
        return null;
    }

    public void RemoveItem(Item item)
    {
        pocket.RemoveItem(item);
    }

    public int NumItems()
    {
        return pocket.GetNumItems();
    }

    public bool CanAddItem()
    {
        return !pocket.IsFull();
    }

    public void UpdateUIInfo()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }

        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        if (expText != null)
        {
            expText.text = exp.ToString();
        }
    }

    public override void Save()
    {
        PlayerData data = new PlayerData(this);
        GameManager.Save(data, base.filename);
    }

    public override void Load()
    {
        base.Load();
        PlayerData data = GameManager.Load<PlayerData>(base.filename);

        if (data != null)
        {
            base.LoadFromData(data);
            this.suspicion = data.suspicion;
            UpdateUIInfo();
        }
    }

    [System.Serializable]
    public class PlayerData : Character.CharacterData
    {
        public int suspicion;

        public PlayerData()
        {
            base.SetPositionalData(Player.START_POS);
            base.SetStats(0, 0, 0, 0);
        }

        public PlayerData(Player player) : base(player)
        {
            this.suspicion = player.suspicion;
        }
    }
}