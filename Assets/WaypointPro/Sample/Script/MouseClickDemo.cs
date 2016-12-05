using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mr1
{
    public class MouseClickDemo : MonoBehaviour
    {
        public Transform target;
        public string pathName;

        void OnMouseUp()
        {
            var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = transform.position.z;
            target.FollowPathToPoint(pathName, targetPos, 10f);
        }
    }
}
