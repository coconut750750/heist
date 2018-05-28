using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimationController : MonoBehaviour {

	public static CharAnimationController instance = null;

	public RuntimeAnimatorController baseAnimator;

	private AnimatorOverrideController overrideAnimator;

	public AnimationClip newForward;
	public AnimationClip newLeft;
	public AnimationClip newBack;
	public AnimationClip newRight;

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
			Debug.Log(anim.name);
			if (anim.name.Contains("forward")) {
				clipOverrides[anim.name] = newForward;
			} else if (anim.name.Contains("left")) {
				clipOverrides[anim.name] = newLeft;
			} else if (anim.name.Contains("back")) {
				clipOverrides[anim.name] = newBack;
			} else if (anim.name.Contains("right")) {
				clipOverrides[anim.name] = newRight;
			}
		}
		overrideAnimator.ApplyOverrides(clipOverrides);
		foreach (AnimationClip anim in overrideAnimator.animationClips) {
			Debug.Log(anim.name);
		}
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