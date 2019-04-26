using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PauseStash : SingletonStash {

    public PauseStash(int numItems) : base(numItems) {

    }

    protected override void Start() {
        base.Start();
    }

    public virtual void Hide() {
        foreach (Item item in items) {
            GameManager.instance.mainPlayer.AddItem(item);
        }
        RemoveAll();
    }

    public override void Load() {
        return;
    }

    public override void Save() {
        return;
    }   
}
