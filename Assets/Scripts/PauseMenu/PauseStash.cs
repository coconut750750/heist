using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PauseStash : SingletonStash {

	public const int NUM_ITEMS = 4;

	protected bool isDisplaying;

    public PauseStash() : base(NUM_ITEMS) {

    }

    protected override void Start() {
        base.Start();
        isDisplaying = false;
    }

    public void Display() {
        SetDisplaying(true);
    }

    public void Hide() {
        foreach (Item item in items) {
            GameManager.instance.mainPlayer.AddItem(item);
            RemoveItem(item);
        }

        SetDisplaying(false);
    }

    public override bool IsDisplaying() 
    {
        return isDisplaying;
    }

    public override void SetDisplaying(bool isDisplaying)
    {
        this.isDisplaying = isDisplaying;
    }

    public override void Load() {
        return;
    }

    public override void Save() {
        return;
    }   
}
