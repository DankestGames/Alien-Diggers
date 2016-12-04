using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mr1
{
    public enum PathLineType
    {
        Straight,
        CatmullRomCurve,
        BezierCurve,
    }

    public class PathData : ScriptableObject
    {
        public bool bShowPath;

        [SerializeField] public string pathName;
        [SerializeField] public PathLineType lineType;
        [SerializeField] public Color lineColor = Color.yellow;
        [SerializeField] public float pointSize = 0.5f;

        [SerializeField] public List<Vector3> points;
        [SerializeField] public List<Vector3> firstHandles; 
        [SerializeField] public List<Vector3> secondHandles;
        [SerializeField] public List<Vector3> linePoints;

        public Vector3 startPoint { get { return linePoints[0]; } }
        public Vector3 endPoint { get { return linePoints[linePoints.Count-1]; } }

        public PathData()
        {
            points = new List<Vector3>();
            firstHandles = new List<Vector3>();
            secondHandles = new List<Vector3>();
            linePoints = new List<Vector3>();
        }

        
    }
}