using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using Mr1;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor 
{
    enum SceneMode
    {
        Add,
        Edit,
    }

    SceneMode sceneMode;
    WaypointManager script;

    #region Menu Method

    [MenuItem("GameObject/Create Waypoint Manager")]
    public static void CreateWaypointManager()
    {
        if (GameObject.FindObjectOfType<WaypointManager>() == null)
        {
            var managerGo = new GameObject("WaypointManager");
            var manager = managerGo.AddComponent<WaypointManager>();
            var boxCollider = managerGo.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100f, 100f, 1f);
            boxCollider.isTrigger = true;
        }
        else
            Debug.LogError("Waypoint Manager already exists!");
    }

    #endregion

    void OnEnable()
    {
        sceneMode = SceneMode.Edit;
        script = target as WaypointManager;
        if (script.pathList == null) script.pathList = new List<PathData>();
        for (int i = 0; i < script.pathList.Count; i++)
        {
            if (script.pathList[i] == null) script.pathList.RemoveAt(i);
        }

        script.selected = null;
        foreach(var path in script.pathList)
        {
            if (path != null)
            {
                String strAssetPath = AssetDatabase.GetAssetPath(path);
                int startIndex = strAssetPath.LastIndexOf("/") + 1;
                int length = strAssetPath.LastIndexOf(".") - startIndex;
                path.pathName = strAssetPath.Substring(startIndex, length);
            }
        }
    }

    void OnDisable()
    {
        AssetDatabase.SaveAssets();
        if (script != null)
        {
            EditorUtility.SetDirty(script);
            foreach (var path in script.pathList)
            {
                if (path != null) EditorUtility.SetDirty(path);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        //base.DrawDefaultInspector();

        ShowButtons();

        CustomGUI.DrawSeparator(Color.gray);

        ShowPath();
        CheckGUIChanged();
    }

    #region OnInspectorGUI's Display Method

    void ShowButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New Path"))
        {
            PathData newPath = CreatePathAsset();
            if (newPath != null) script.pathList.Add(newPath);
        }
        if (GUILayout.Button("Load Path"))
        {
            PathData loadedPath = LoadPathAsset();
            if (loadedPath != null)
            {
                if (!script.pathList.Contains(loadedPath))
                    script.pathList.Add(loadedPath);
            }
        }

        GUILayout.EndHorizontal();
    }

    void ShowPath()
    {
        for (int i = 0; i < script.pathList.Count; i++)
        {
            Action delAction = () => { script.pathList.RemoveAt(i); };
            if (script.pathList[i] == null || string.IsNullOrEmpty(script.pathList[i].pathName)) continue;
            if (CustomGUI.HeaderButton(script.pathList[i].pathName, null, delAction))
            {
                script.selected = script.pathList[i];
                script.selected.pointSize = EditorGUILayout.Slider("Point Size", script.selected.pointSize, 0.1f, 3f);
                script.selected.lineColor = EditorGUILayout.ColorField("Path Color", script.selected.lineColor);
                script.selected.lineType = (PathLineType)EditorGUILayout.EnumPopup("Path Type", script.selected.lineType);

                List<Vector3> wayPointList = script.selected.points;

                if (wayPointList.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        float fDepth = EditorGUILayout.FloatField("Path Depth", wayPointList[0].z);
                        if (wayPointList[0].z != fDepth) SetWaypointDepth(fDepth);
                
                        if (GUILayout.Button("Depth +"))
                            SetWaypointDepth(fDepth + 1);
                        if (GUILayout.Button("Depth -"))
                            SetWaypointDepth(fDepth - 1);
                    }
                    GUILayout.EndHorizontal();
                }
                
                int count = 0;
                for (int j = 0; j < wayPointList.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        wayPointList[j] = EditorGUILayout.Vector3Field("Point " + count++, wayPointList[j]);
                        if (GUILayout.Button("+", GUILayout.Width(25f)))
                            AddWaypoint(wayPointList[j] + Vector3.right + Vector3.up, j + 1);
                        if (GUILayout.Button("-", GUILayout.Width(25f)))
                            DeleteWaypoint(j);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    #endregion

    #region Path Asset Method

    PathData CreatePathAsset()
    {
        string strAssetPath = EditorUtility.SaveFilePanelInProject("New Path", "NewPath", "asset", "");
        
        if (string.IsNullOrEmpty(strAssetPath))
            return null;

        strAssetPath = AssetDatabase.GenerateUniqueAssetPath(strAssetPath);

        int startIndex = strAssetPath.LastIndexOf("/") + 1;
        int length = strAssetPath.LastIndexOf(".") - startIndex;
        string strAssetName = strAssetPath.Substring(startIndex, length);

        PathData newPath = ScriptableObject.CreateInstance<PathData>();
        newPath.pathName = strAssetName;

        AssetDatabase.CreateAsset(newPath, strAssetPath);
        AssetDatabase.SaveAssets();

        return newPath;
    }

    PathData LoadPathAsset()
    {
        string strAssetPath = EditorUtility.OpenFilePanel("Load Path", "Assets/", "asset");
        strAssetPath = strAssetPath.Substring(strAssetPath.IndexOf("Assets/"));

        if (string.IsNullOrEmpty(strAssetPath))
            return null;
        
        PathData loadedPath = (PathData)AssetDatabase.LoadAssetAtPath(strAssetPath, typeof(PathData));

        int startIndex = strAssetPath.LastIndexOf("/") + 1;
        int length = strAssetPath.LastIndexOf(".") - startIndex;
        string strAssetName = strAssetPath.Substring(startIndex, length);

        loadedPath.pathName = strAssetName;

        return loadedPath;
    }
    
    void SavePathAsset()
    {
        AssetDatabase.SaveAssets();
        foreach (var path in script.pathList)
            EditorUtility.SetDirty(path);
    }

    #endregion

    void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        if (script.selected != null)
        {
            DrawWindow();

            if (sceneMode == SceneMode.Add)
            {
                UpdateMouseInput();
                DrawWaypoint();
            }
            else if (sceneMode == SceneMode.Edit)
            {
                DrawHandlePoint();
            }

            DrawPathLine();
        }

        CheckGUIChanged();
    }

    #region Input Method

    void UpdateMouseInput()
    {
        Event e = Event.current;
        if (e.type == EventType.mouseDown)
        {
            if (e.button == 0)
                OnMouseClick(e.mousePosition);
        }
    }

    void OnMouseClick(Vector2 mousePos)
    {
        LayerMask backgroundLayerMask = 1 << script.gameObject.layer;

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, backgroundLayerMask))
        {
            Vector3 hitPos = hit.point;
            AddWaypoint(hitPos);
        }
    }

    #endregion

    #region OnSceneGUI's Draw Method

    void DrawWindow()
    {
        GUILayout.Window(1, new Rect(0f, 25f, 70f, 80f), DoWaypointWindow, script.selected.pathName);
    }

    void DoWaypointWindow(int windowID)
    {
        EditorGUILayout.BeginVertical();

        sceneMode = (SceneMode)GUILayout.SelectionGrid((int)sceneMode, System.Enum.GetNames(typeof(SceneMode)), 1);

        if (GUILayout.Button("Del"))
            DeleteWaypoint();

        if (GUILayout.Button("Clear"))
            ClearWaypoint();

        GUI.color = Color.green;
        script.selected.lineType = (PathLineType)EditorGUILayout.EnumPopup(script.selected.lineType);
        GUI.color = Color.white;

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            SetLinePoints();
        }

        //GUI.DragWindow();
    }

    void DrawWaypoint()
    {
        Handles.color = Color.green;
        foreach (var point in script.selected.points)
        {
            Handles.SphereCap(0, point, Quaternion.identity, script.selected.pointSize);
        }
        Handles.color = Color.white;
    }

    void DrawPathLine()
    {
        List<Vector3> linePoints = script.selected.linePoints;
        if (linePoints == null) return;

        Handles.color = script.selected.lineColor;
        for (int i = 1; i < linePoints.Count; i++)
        {
            Handles.DrawLine(linePoints[i - 1], linePoints[i]);
        }
        Handles.color = Color.white;
    }

    void DrawHandlePoint()
    {
        List<Vector3> wayPoints = script.selected.points;

        for (int i = 0; i < wayPoints.Count; i++)
        {
            Handles.color = Color.magenta;
            wayPoints[i] = Handles.FreeMoveHandle(wayPoints[i], Quaternion.identity, script.selected.pointSize, Vector3.zero, Handles.SphereCap);

            if (script.selected.lineType == PathLineType.BezierCurve)
            {
                Vector3 firstControlPoint = wayPoints[i] + script.selected.firstHandles[i];
                Vector3 secondControlPoint = wayPoints[i] + script.selected.secondHandles[i];

                Handles.color = Color.gray;
                if (i != 0)
                {
                    Vector3 movedPoint = Handles.FreeMoveHandle(firstControlPoint, Quaternion.identity, script.selected.pointSize, Vector3.zero, Handles.SphereCap);
                    if (firstControlPoint != movedPoint)
                    {
                        firstControlPoint = movedPoint - wayPoints[i];

                        Quaternion qRot = Quaternion.FromToRotation(script.selected.firstHandles[i], firstControlPoint);
                        script.selected.secondHandles[i] = qRot * script.selected.secondHandles[i];
                        script.selected.firstHandles[i] = firstControlPoint;
                    }
                    Handles.DrawLine(wayPoints[i], firstControlPoint);
                }
                if (i != wayPoints.Count - 1)
                {
                    Vector3 movedPoint = Handles.FreeMoveHandle(secondControlPoint, Quaternion.identity, script.selected.pointSize, Vector3.zero, Handles.SphereCap);
                    if (secondControlPoint != movedPoint)
                    {
                        secondControlPoint = movedPoint - wayPoints[i];

                        Quaternion qRot = Quaternion.FromToRotation(script.selected.secondHandles[i], secondControlPoint);
                        script.selected.firstHandles[i] = qRot * script.selected.firstHandles[i];
                        script.selected.secondHandles[i] = secondControlPoint;
                    }
                    Handles.DrawLine(wayPoints[i], secondControlPoint);
                }
                Handles.color = Color.white;
            }
        }
        Handles.color = Color.white;
    }

    #endregion

    #region Line Points Setting Method

    void SetLinePoints()
    {
        if (script.selected == null) return;

        if (script.selected.linePoints == null)
            script.selected.linePoints = new List<Vector3>();
        else
            script.selected.linePoints.Clear();

        switch (script.selected.lineType)
        {
            case PathLineType.Straight: SetStraightLine(); break;
            case PathLineType.CatmullRomCurve: SetCatmullRomCurveLine(); break;
            case PathLineType.BezierCurve: SetBezierCurveLine(); break;
        }

        SceneView.RepaintAll();
    }

    void SetStraightLine()
    {
        List<Vector3> wayPoints = script.selected.points;
        if (wayPoints.Count < 2)
            return;

        for (int i = 0; i < wayPoints.Count-1; i++)
        {
            for (float t = 0f; t <= 1.0f; t += 0.05f)
            {
                Vector3 pt = wayPoints[i] * (1f - t) + wayPoints[i + 1] * t;
                script.selected.linePoints.Add(pt);
            }
        }

        script.selected.linePoints.Add(wayPoints[wayPoints.Count - 1]);
    }

    void SetCatmullRomCurveLine()
    {
        List<Vector3> wayPoints = script.selected.points;

        if (wayPoints.Count < 3)
            return;

        Vector3[] catmullRomPoints = new Vector3[wayPoints.Count + 2];
        wayPoints.CopyTo(catmullRomPoints, 1);

        int endIndex = catmullRomPoints.Length - 1;

        catmullRomPoints[0] = catmullRomPoints[1] + (catmullRomPoints[1] - catmullRomPoints[2]) + (catmullRomPoints[3] - catmullRomPoints[2]);
        catmullRomPoints[endIndex] = catmullRomPoints[endIndex - 1] + (catmullRomPoints[endIndex - 1] - catmullRomPoints[endIndex - 2])
                                + (catmullRomPoints[endIndex - 3] - catmullRomPoints[endIndex - 2]);

        script.selected.linePoints.Add(wayPoints[0]);

        for (int i = 0; i < catmullRomPoints.Length - 3; i++)
        {
            for (float t = 0.05f; t <= 1.0f; t += 0.05f)
            {
                Vector3 pt = ComputeCatmullRom(catmullRomPoints[i], catmullRomPoints[i + 1], catmullRomPoints[i + 2], catmullRomPoints[i + 3], t);
                script.selected.linePoints.Add(pt);
            }
        }

        script.selected.linePoints.Add(wayPoints[wayPoints.Count - 1]);
    }

    Vector3 ComputeCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 pt = 0.5f * ((-p0 + 3f * p1 - 3f * p2 + p3) * t3
                    + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2
                    + (-p0 + p2) * t
                    + 2f * p1);

        return pt;
    }

    void SetBezierCurveLine()
    {
        List<Vector3> wayPoints = script.selected.points;
        List<Vector3> firstControls = script.selected.firstHandles;
        List<Vector3> secondControls = script.selected.secondHandles;

        if (wayPoints.Count < 2)
            return;

        script.selected.linePoints.Add(wayPoints[0]);

        for (int i = 0; i < wayPoints.Count - 1; i++)
        {
            Vector3 waypoint1 = wayPoints[i];
            Vector3 waypoint2 = wayPoints[i + 1];
            Vector3 controlPoint1 = wayPoints[i] + secondControls[i];
            Vector3 controlPoint2 = wayPoints[i + 1] + firstControls[i + 1];

            for (float t = 0.05f; t <= 1.0f; t += 0.05f)
            {
                Vector3 pt = ComputeBezier(waypoint1, controlPoint1, controlPoint2, waypoint2, t);

                script.selected.linePoints.Add(pt);
            }
        }

        script.selected.linePoints.Add(wayPoints[wayPoints.Count-1]);
    }

    Vector3 ComputeBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t * t * t;
        float _t2 = (1 - t) * (1 - t);
        float _t3 = (1 - t) * (1 - t) * (1 - t);

        return p0 * _t3 + 3f * p1 * t * _t2 + 3 * p2 * t2 * (1 - t) + p3 * t3;
    }

    #endregion

    #region Waypoint Method

    void AddWaypoint(Vector3 position, int addIndex = -1)
    {
        position.z = (script.selected.points.Count > 0) ? script.selected.points[0].z : 0f;

        if (addIndex == -1)
            script.selected.points.Add(position);
        else
            script.selected.points.Insert(addIndex, position);

        script.selected.firstHandles.Add(Vector3.left);
        script.selected.secondHandles.Add(Vector3.right);
        SetLinePoints();
    }

    void DeleteWaypoint(int removeIndex = -1)
    {
        List<Vector3> wayPointList = script.selected.points;
        if (wayPointList == null || wayPointList.Count == 0)
            return;

        if (removeIndex == -1) removeIndex = wayPointList.Count - 1;
        wayPointList.RemoveAt(removeIndex);
        script.selected.firstHandles.RemoveAt(removeIndex);
        script.selected.secondHandles.RemoveAt(removeIndex);
        SetLinePoints();
    }

    void ClearWaypoint()
    {
        script.selected.points.Clear();
        script.selected.firstHandles.Clear();
        script.selected.secondHandles.Clear();
        SetLinePoints();
    }

    void SetWaypointDepth(float fDepth)
    {
        List<Vector3> wayPointList = script.selected.points;
        for (int i = 0; i < wayPointList.Count; i++)
            wayPointList[i] = new Vector3(wayPointList[i].x, wayPointList[i].y, fDepth);

        SetLinePoints();
    }

    #endregion

    void CheckGUIChanged()
    {
        if (GUI.changed)
        {
            SetLinePoints();
            AssetDatabase.SaveAssets();
            SceneView.RepaintAll();
        }
    }
}
