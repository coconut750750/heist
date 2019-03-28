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

    public void Hide() {
        foreach (Item item in items) {
            GameManager.instance.mainPlayer.AddItem(item);
            RemoveItem(item);
        }
    }

    public override void Load() {
        return;
    }

    public override void Save() {
        return;
    }   
}
