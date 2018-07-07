using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PauseStash : SingletonStash {

	protected bool isDisplaying;

    public PauseStash(int numItems) : base(numItems) {

    }

    protected override void Start() {
        base.Start();
        isDisplaying = false;
    }

    public void Display() {
        SetDisplaying(true);
    }

    public void Hide() {
        SetDisplaying(false);
        foreach (Item item in items) {
            GameManager.instance.mainPlayer.AddItem(item);
            RemoveItem(item);
        }
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
