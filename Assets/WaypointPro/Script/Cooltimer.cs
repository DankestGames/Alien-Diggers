using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mr1
{
    public class Cooltimer : MonoBehaviour
    {
        public float cooltime;
        public Action onFinished;

        public static Cooltimer Set(Component component, float cooltime, Action onFinished)
        {
            var cooltimer = component.GetComponent<Cooltimer>();
            if (cooltimer == null) cooltimer = component.gameObject.AddComponent<Cooltimer>();
            cooltimer.cooltime = cooltime;
            cooltimer.onFinished = onFinished;
            return cooltimer;
        }

        void Update()
        {
            cooltime = Mathf.Max(0f, cooltime - Time.smoothDeltaTime);
            if (cooltime <= 0f)
            {
                if (onFinished != null) onFinished();
                GameObject.Destroy(this);
            }
        }
    }
}
