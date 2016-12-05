using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mr1
{
    public enum FollowType { Once, Loop, PingPong, }
    public enum FollowDirection { Forward, Backward, }

    public class PathFollower : MonoBehaviour
    {
        public bool logMessage;
        public PathData pathData;
        public FollowType followType;
        public FollowDirection followDirection;
        public float moveSpeed = 10f;
        public float rotateSpeed = 10f;

        [SerializeField] bool _flip;
        [SerializeField] bool _lookForward;
        [SerializeField] bool _smoothLookForward;
        Transform _transform;
        int _currentIndex;

        public static PathFollower Create(Transform transform)
        {
            var pathFollower = transform.GetComponent<PathFollower>();
            if (pathFollower == null) pathFollower = transform.gameObject.AddComponent<PathFollower>();
            pathFollower._transform = transform;
            return pathFollower;
        }

        public static void Stop(Transform transform)
        {
            var pathFollower = transform.GetComponent<PathFollower>();
            if (pathFollower != null) { pathFollower.StopFollowing(); GameObject.Destroy(pathFollower); }
        }

        public void SetFlip(bool useFlip) { _lookForward = _smoothLookForward = false; _flip = useFlip; }
        public void SetLookForward(bool useLookforward) { _flip = _smoothLookForward = false; _lookForward = useLookforward; }
        public void SetSmoothLookForward(bool useSmoothLookforward, float rotSpeed) { _flip = _lookForward = false; _smoothLookForward = useSmoothLookforward; rotateSpeed = rotSpeed; }
        public void Follow(PathData pathData, float moveSpeed, FollowType followType, FollowDirection followDirection)
        {
            this.pathData = pathData;
            this.moveSpeed = moveSpeed;
            this.followType = followType;
            this.followDirection = followDirection;

            StopFollowing();

            var closestLineIndex = GetClosestLineIndex(_transform.position);
            _currentIndex = GetClosestPointIndex(closestLineIndex * 20, _transform.position);
            StartCoroutine("FollowPath");
        }
        public void FollowToPoint(PathData pathData, float moveSpeed, Vector2 targetPos)
        {
            this.pathData = pathData;
            this.moveSpeed = moveSpeed;
            this.followType = FollowType.Once;

            StopFollowing();

            var closestLineIndex = GetClosestLineIndex(_transform.position);
            _currentIndex = GetClosestPointIndex(closestLineIndex * 20, _transform.position);
            closestLineIndex = GetClosestLineIndex(targetPos);
            var targetIndex = GetClosestPointIndex(closestLineIndex * 20, targetPos);
            this.followDirection = (_currentIndex < targetIndex) ? FollowDirection.Forward : FollowDirection.Backward;
            StartCoroutine("FollowPathToPoint", targetIndex);
        }

        public void StopFollowing() { StopCoroutine("FollowPath"); StopCoroutine("FollowPathToPoint"); }
        
        IEnumerator FollowPath()
        {
            yield return null;
            if (logMessage) Debug.Log(string.Format("[{0}] Follow() Path:{1}, Type:{2}, Speed:{3}", name, pathData.name, followType, moveSpeed));

            while (true)
            {
                _currentIndex = Mathf.Clamp(_currentIndex, 0, pathData.linePoints.Count - 1);

                if (IsOnPoint(_currentIndex))
                {
                    if (IsEndPoint(_currentIndex)) break;
                    else _currentIndex = GetNextIndex(_currentIndex);
                }
                else
                {
                    MoveTo(_currentIndex);
                }
                yield return null;
            }
        }
        IEnumerator FollowPathToPoint(int targetIndex)
        {
            yield return null;
            if (logMessage) Debug.Log(string.Format("[{0}] FollowToPoint() Path:{1}, Speed:{2}", name, pathData.name, moveSpeed));

            targetIndex = Mathf.Clamp(targetIndex, 0, pathData.linePoints.Count - 1);

            while (!IsOnPoint(targetIndex))
            {
                _currentIndex = Mathf.Clamp(_currentIndex, 0, pathData.linePoints.Count - 1);

                if (IsOnPoint(_currentIndex))
                {
                    if (IsEndPoint(_currentIndex)) break;
                    else _currentIndex = GetNextIndex(_currentIndex);
                }
                else
                {
                    MoveTo(_currentIndex);
                }
                yield return null;
            }
        }

        void MoveTo(int pointIndex)
        {
            var targetPos = pathData.linePoints[pointIndex];

            if (_flip)
            {
                var deltaPos = targetPos - _transform.position;
                _transform.right = (deltaPos.x >= 0f) ? Vector3.right : Vector3.left;
            }
            else if (_lookForward)
            {
                var deltaPos = targetPos - _transform.position;
                deltaPos.z = 0f;
                _transform.up = deltaPos;
            }
            else if (_smoothLookForward)
            {
                var deltaPos = targetPos - _transform.position;
                deltaPos.z = 0f;
                _transform.up = Vector3.Lerp(_transform.up, deltaPos.normalized, rotateSpeed * Time.smoothDeltaTime);
            }
            _transform.position = Vector3.MoveTowards(_transform.position, targetPos, moveSpeed * Time.smoothDeltaTime);
        }

        bool IsOnPoint(int pointIndex) { return (_transform.position - pathData.linePoints[pointIndex]).sqrMagnitude < 0.1f; }
        bool IsEndPoint(int pointIndex)
        {
            switch (followType)
            {
                case FollowType.Once: return pointIndex == EndIndex();
                case FollowType.Loop: return false;
                case FollowType.PingPong: return false;
            }
            return false;
        }

        int StartIndex()
        {
            if (followDirection == FollowDirection.Forward) return 0;
            return pathData.linePoints.Count - 1;
        }
        int EndIndex()
        {
            if (followDirection == FollowDirection.Backward) return 0;
            return pathData.linePoints.Count - 1;
        }

        int GetNextIndex(int currentIndex)
        {
            int nextIndex = -1;
            switch (followType)
            {
                case FollowType.Once:
                    if (followDirection == FollowDirection.Forward)
                    {
                        if (currentIndex < EndIndex()) nextIndex = currentIndex + 1;
                    }
                    else if (followDirection == FollowDirection.Backward)
                    {
                        if (currentIndex > EndIndex()) nextIndex = currentIndex - 1;
                    }
                    break;
                case FollowType.Loop:
                    if (followDirection == FollowDirection.Forward)
                    {
                        if (currentIndex < EndIndex()) nextIndex = currentIndex + 1;
                        else nextIndex = StartIndex();
                    }
                    else if (followDirection == FollowDirection.Backward)
                    {
                        if (currentIndex > EndIndex()) nextIndex = currentIndex - 1;
                        else nextIndex = StartIndex();
                    }
                    break;
                case FollowType.PingPong:
                    if (followDirection == FollowDirection.Forward)
                    {
                        if (currentIndex < EndIndex()) nextIndex = currentIndex + 1;
                        else
                        {
                            followDirection = FollowDirection.Backward;
                            nextIndex = currentIndex - 1;
                        }
                    }
                    else if (followDirection == FollowDirection.Backward)
                    {
                        if (currentIndex > EndIndex()) nextIndex = currentIndex - 1;
                        else
                        {
                            followDirection = FollowDirection.Forward;
                            nextIndex = currentIndex + 1;
                        }
                    }
                    break;
            }
            return nextIndex;
        }

        int GetClosestLineIndex(Vector3 pos)
        {
            List<Vector3> wayPoints = pathData.points;

            Vector3 vClosestPoint = ComputeClosestPointFromPointToLine(pos, wayPoints[0], wayPoints[1]);
            float fClosestDist = (vClosestPoint - pos).sqrMagnitude;
            int nLineIndex = 0;

            for (int i = 1; i < wayPoints.Count - 1; i++)
            {
                Vector3 vPos = ComputeClosestPointFromPointToLine(pos, wayPoints[i], wayPoints[i + 1]);
                float fDist = (vPos - pos).sqrMagnitude;

                if (fDist < fClosestDist)
                {
                    fClosestDist = fDist;
                    vClosestPoint = vPos;
                    nLineIndex = i;
                }
            }

            //Debug.DrawLine(pos, vClosestPoint, Color.green, 1f);
            return nLineIndex;
        }

        int GetClosestPointIndex(int nStartIndex, Vector3 pos)
        {
            int nLastIndex = Mathf.Min(nStartIndex + 20, pathData.linePoints.Count - 1);
            Vector3 vClosestPos = pathData.linePoints[nStartIndex];
            float fClosestDist = (vClosestPos - pos).sqrMagnitude;
            int nPointIndex = nStartIndex;

            for (int i = nStartIndex + 1; i <= nLastIndex; i++)
            {
                Vector3 vPos = pathData.linePoints[i];
                float fDist = (vPos - pos).sqrMagnitude;

                if (fDist < fClosestDist)
                {
                    fClosestDist = fDist;
                    vClosestPos = vPos;
                    nPointIndex = i;
                }
            }

            // Debug.DrawLine(pos, vClosestPos, Color.yellow, 1f);
            return nPointIndex;
        }

        Vector3 ComputeClosestPointFromPointToLine(Vector3 vPt, Vector3 vLinePt0, Vector3 vLinePt1)
        {
            float t = -Vector3.Dot(vPt - vLinePt0, vLinePt1 - vLinePt0) / Vector3.Dot(vLinePt0 - vLinePt1, vLinePt1 - vLinePt0);

            Vector3 vClosestPt;

            if (t < 0f)
                vClosestPt = vLinePt0;
            else if (t > 1f)
                vClosestPt = vLinePt1;
            else
                vClosestPt = vLinePt0 + t * (vLinePt1 - vLinePt0);

            //Debug.DrawLine(vPt, vClosestPt, Color.red, 1f);
            return vClosestPt;
        }
    }
}
