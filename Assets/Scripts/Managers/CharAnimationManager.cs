using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimationManager : MonoBehaviour {

	private const string FORWARD = "forward";
	private const string LEFT = "left";
	private const string BACK = "back";
	private const string RIGHT = "right";

	public static CharAnimationManager instance = null;

	public RuntimeAnimatorController baseAnimator;

	private AnimatorOverrideController overrideAnimator;

	public AnimationClip newBack;
	public AnimationClip newForward;
	public AnimationClip newLeft;
	public AnimationClip newRight;

	public Sprite[] newBackSprites;
	public Sprite[] newForwardSprites;
	public Sprite[] newLeftSprites;
	public Sprite[] newRightSprites;

	void Start () {
		if (instance != null) {
			Destroy(this);
		} else {
			instance = this;
		}

		overrideAnimator = new AnimatorOverrideController(baseAnimator);

		AnimationClipOverrides clipOverrides = new AnimationClipOverrides(overrideAnimator.overridesCount);
		overrideAnimator.GetOverrides(clipOverrides);

		foreach (AnimationClip anim in overrideAnimator.animationClips) {
			if (anim.name.Contains(FORWARD)) {
				clipOverrides[anim.name] = newForward;
			} else if (anim.name.Contains(LEFT)) {
				clipOverrides[anim.name] = newLeft;
			} else if (anim.name.Contains(BACK)) {
				clipOverrides[anim.name] = newBack;
			} else if (anim.name.Contains(RIGHT)) {
				clipOverrides[anim.name] = newRight;
			}
		}
		overrideAnimator.ApplyOverrides(clipOverrides);
	}

	public RuntimeAnimatorController GetNewAnimator() {
		return overrideAnimator;
	}
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) {}

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}