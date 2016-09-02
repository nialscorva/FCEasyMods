using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialscorva.FCEEasyMods
{
    using StateCoroutine = System.Collections.Generic.IEnumerator<ModMachineEntity.StateReturn>;
    using Coroutine = System.Collections.Generic.IEnumerator<int>;
    using UnityEngine;
    public class AnimationCoroutines
    {
        public static Coroutine ShrinkAway(GameObject go,float duration,Action onComplete)
        {
            Vector3 scale = go.transform.localScale;
            float factor = 1.0f;
            float step = duration / 10;
            while (factor >= 0)
            {
                factor -= step;
                go.transform.localScale = factor*scale;
                yield return 10;
            }
            onComplete();
        }
    }
}
