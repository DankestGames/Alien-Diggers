using UnityEngine;
using System.Collections;

namespace Mr1
{
    public class PlayerDemo : MonoBehaviour
    {
        void Awake()
        {
            transform.FollowPath("First", 10f, FollowType.Loop).Log(true);
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            transform.FollowPath("First", 10f, FollowType.Loop).Duration(3f).LookForward(true);
        
            yield return new WaitForSeconds(4f);
            transform.FollowPath("First", 20f, FollowType.PingPong, FollowDirection.Backward).Flip(true);
        
            yield return new WaitForSeconds(5f);
            transform.FollowPath("Second", 13f, FollowType.PingPong).SmoothLookForward(true, 10f);
        }
    }

}