using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mr1;

namespace Mr1
{
    public class WaypointManager : MonoBehaviour
    {
        static WaypointManager _instance;
        public static WaypointManager instance { get { if (_instance == null) { _instance = GameObject.FindObjectOfType<WaypointManager>(); _instance.Init(); } return _instance; } }
        public PathData selected { get; set; }

        public List<PathData> pathList = new List<PathData>();
        Dictionary<string, PathData> _pathDic;

        void Init()
        {
            GetComponent<Collider>().enabled = false;
            _pathDic = new Dictionary<string, PathData>(pathList.Count);
            for (int i = 0; i < pathList.Count; ++i) _pathDic.Add(pathList[i].pathName, pathList[i]);
        }

        public PathData GetPathData(string pathName)
        {
            if (_pathDic.ContainsKey(pathName)) return _pathDic[pathName];
            return null;
        }
    }
}

public static class WaypointProExtensions
{
    public static PathFollower FollowPath(this Transform transform, string pathName, float moveSpeed,
                                    FollowType followType = FollowType.Once, FollowDirection followDirection = FollowDirection.Forward)
    {
        PathData pathData = WaypointManager.instance.GetPathData(pathName);
        var pathFollower = PathFollower.Create(transform);
        if (pathData != null) pathFollower.Follow(pathData, moveSpeed, followType, followDirection);
        else Debug.LogError(string.Format("[WaypointManager] couldn't find path('{0}')", pathName));
        return pathFollower;
    }
    public static PathFollower FollowPathToPoint(this Transform transform, string pathName, Vector2 targetPos, float moveSpeed)
    {
        PathData pathData = WaypointManager.instance.GetPathData(pathName);
        var pathFollower = PathFollower.Create(transform);
        if (pathData != null) pathFollower.FollowToPoint(pathData, moveSpeed, targetPos);
        else Debug.LogError(string.Format("[WaypointManager] couldn't find path('{0}')", pathName));
        return pathFollower;
    }
    public static void StopFollowing(this Transform transform)
    {
        PathFollower.Stop(transform);
    }
    public static PathFollower Duration(this PathFollower pathFollower, float duration)
    {
        Cooltimer.Set(pathFollower, duration, () => pathFollower.StopFollowing());
        return pathFollower;
    }
    public static PathFollower Flip(this PathFollower pathFollower, bool useFlip)
    {
        pathFollower.SetFlip(useFlip);
        return pathFollower;
    }
    public static PathFollower LookForward(this PathFollower pathFollower, bool useLookForward)
    {
        pathFollower.SetLookForward(useLookForward);
        return pathFollower;
    }
    public static PathFollower SmoothLookForward(this PathFollower pathFollower, bool useSmoothLookForward, float rotateSpeed)
    {
        pathFollower.SetSmoothLookForward(useSmoothLookForward, rotateSpeed);
        return pathFollower;
    }
    public static PathFollower Log(this PathFollower pathFollower, bool logMessage)
    {
        pathFollower.logMessage = logMessage;
        return pathFollower;
    }
}