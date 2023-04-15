namespace ExtendedColliders3DEditor {
    using ExtendedColliders3D;
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ExtendedColliders3D))]
    public class ExtendedColliders3DEditor : Editor {

        //Variables.
        static string lastUndoEvent = "";
        static int undoGroup = -1;
        bool basePositionRotationOrScaleDirty = true;
        Vector3 baseGlobalPosition = Vector3.zero;
        Vector3 basePositionRotationOrScale = Vector3.zero;
        Vector3 previousTransformPosition = Vector3.zero;
        Vector3 previousTransformRotation = Vector3.zero;
        Vector3 previousTransformScale = Vector3.one;

        //On GUI.
        public override void OnInspectorGUI() {

            //Get the Extended Colliders 3D instance.
            ExtendedColliders3D extendedColliders3D = (ExtendedColliders3D) target;

            //Check whether the transform has moved since the last frame - if so, flag the base position, rotation and scale as dirty.
            if (Vector3.Distance(previousTransformPosition, extendedColliders3D.transform.position) > 0.0001f ||
                    Vector3.Distance(previousTransformRotation, extendedColliders3D.transform.eulerAngles) > 0.0001f ||
                    Vector3.Distance(previousTransformScale, extendedColliders3D.transform.lossyScale) > 0.0001f) {
                basePositionRotationOrScaleDirty = true;
                previousTransformPosition = extendedColliders3D.transform.position;
                previousTransformRotation = extendedColliders3D.transform.eulerAngles;
                previousTransformScale = extendedColliders3D.transform.lossyScale;
            }

            //Mesh collider settings header.
            EditorGUILayout.GetControlRect();
            EditorGUILayout.LabelField("Mesh Collider Settings", EditorStyles.boldLabel);

            //Convex flag.
            EditorGUI.BeginChangeCheck();
            bool convex = EditorGUILayout.Toggle(new GUIContent("Convex", "Is this collider convex?"), extendedColliders3D.properties.convex);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Toggle Extended Colliders 3D convex flag");
                extendedColliders3D.properties.convex = convex;
                endUndo(true);
            }

            //Is trigger flag.
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!convex);
            EditorGUI.BeginChangeCheck();
            bool isTrigger = EditorGUILayout.Toggle(new GUIContent("Is Trigger", "Is this collider a trigger? Triggers are only supported on convex colliders."),
                    extendedColliders3D.properties.isTrigger);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Toggle Extended Colliders 3D is trigger flag");
                extendedColliders3D.properties.isTrigger = isTrigger;
                endUndo(true);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            //Material.
            EditorGUI.BeginChangeCheck();
            PhysicMaterial material = (PhysicMaterial) EditorGUILayout.ObjectField(new GUIContent("Material"), extendedColliders3D.properties.material,
                    typeof(PhysicMaterial), false);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Toggle Extended Colliders 3D Material");
                extendedColliders3D.properties.material = material;
                endUndo(true);
            }

            //General collider settings header.
            EditorGUILayout.GetControlRect();
            EditorGUILayout.LabelField("General Collider Settings", EditorStyles.boldLabel);

            //Collider type.
            EditorGUI.BeginChangeCheck();
            ExtendedColliders3D.ColliderType colliderType = (ExtendedColliders3D.ColliderType) EditorGUILayout.EnumPopup(
                    new GUIContent("Collider Type", "The type of 3D primitive shape to create."), extendedColliders3D.properties.colliderType);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Collider Type");
                extendedColliders3D.properties.colliderType = colliderType;
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }

            //Centre.
            EditorGUI.BeginChangeCheck();
            Vector3 centre = EditorGUILayout.Vector3Field(new GUIContent("Centre", "The centre point of the collider."), extendedColliders3D.properties.centre);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Centre Point");
                extendedColliders3D.properties.centre = centre;
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }

            //Rotation.
            EditorGUI.BeginChangeCheck();
            Vector3 rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation", "The rotation of the collider as Euler angles."),
                    extendedColliders3D.properties.rotation);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Rotation");
                extendedColliders3D.properties.rotation = rotation;
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }

            //Size.
            EditorGUI.BeginChangeCheck();
            Vector3 size = EditorGUILayout.Vector3Field(new GUIContent("Scale", "The scale of the collider."), extendedColliders3D.properties.size);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Scale");
                extendedColliders3D.properties.size = size;
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }

            //Button to auto-size the collider to the mesh, if one is present.
            MeshFilter meshFilter = extendedColliders3D.gameObject.GetComponent<MeshFilter>();
            Mesh mesh = null;
            if (meshFilter != null)
                mesh = meshFilter.sharedMesh;
            bool disabled = mesh == null;
            EditorGUI.BeginDisabledGroup(disabled);
            Rect rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect, new GUIContent("Auto-size to Mesh", disabled ? "Add a mesh filter component with an associated mesh to enable auto-sizing." :
                    "Change the centre and scale of the collider to match the mesh associated with this mesh filter."))) {
                beginUndo("Auto-size Extended Collider");
                extendedColliders3D.autoSizeColliderToMeshFilter();
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }
            EditorGUI.EndDisabledGroup();

            //Flip faces.
            EditorGUI.BeginChangeCheck();
            bool flipFaces = EditorGUILayout.Toggle(new GUIContent("Flip faces", "Whether to flip the collider faces. Doing so turns the collider inside-out, " +
                    "meaning objects will collide with, for example, the inside of cylinder instead of the outside."), extendedColliders3D.properties.flipFaces);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Flip Faces Flag");
                extendedColliders3D.properties.flipFaces = flipFaces;
                endUndo(true);
            }

            //Collider-specific settings header.
            EditorGUILayout.GetControlRect();
            EditorGUILayout.LabelField(new string[] { "Circle", "Circle - Half", "Cone", "Cone - Half", "Cube", "Cylinder", "Cylinder - Half", "Quad",
                "Triangle", "Sphere" }[(int) colliderType] + " Settings", EditorStyles.boldLabel);

            //(Half) circle-specific settings.
            if (colliderType == ExtendedColliders3D.ColliderType.Circle || colliderType == ExtendedColliders3D.ColliderType.CircleHalf) {

                //Circle vertices.
                EditorGUI.BeginChangeCheck();
                int circleVertices = Math.Max(Math.Min(EditorGUILayout.IntField(new GUIContent("Vertices",
                        "The number of vertices the circle will have. More vertices mean a smoother circle."),
                        extendedColliders3D.properties.circleVertices), 256), 3);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Circle Vertices");
                    extendedColliders3D.properties.circleVertices = circleVertices;
                    endUndo(true);
                }

                //Two-sided circle.
                EditorGUI.BeginChangeCheck();
                bool circleTwoSided = EditorGUILayout.Toggle(new GUIContent("Two-sided", "Whether the circle should allow collisions from both sides."),
                        extendedColliders3D.properties.circleTwoSided);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Circle Two-Sided Flag");
                    extendedColliders3D.properties.circleTwoSided = circleTwoSided;
                    endUndo(true);
                }
            }

            //(Half) cone-specific settings.
            if (colliderType == ExtendedColliders3D.ColliderType.Cone || colliderType == ExtendedColliders3D.ColliderType.ConeHalf) {

                //Cone faces.
                EditorGUI.BeginChangeCheck();
                int coneFaces = Math.Max(Math.Min(EditorGUILayout.IntField(new GUIContent("Faces",
                        "The number of faces the cone will have. More faces mean a smoother surface."), extendedColliders3D.properties.coneFaces), 256), 3);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cone Faces");
                    extendedColliders3D.properties.coneFaces = coneFaces;
                    endUndo(true);
                }

                //Cap top.
                EditorGUI.BeginChangeCheck();
                bool coneCap = EditorGUILayout.Toggle(new GUIContent("Cap", "Whether to add a cap onto the bottom of the cone."),
                        extendedColliders3D.properties.coneCap);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cone Cap Flag");
                    extendedColliders3D.properties.coneCap = coneCap;
                    endUndo(true);
                }

                //Cap flat end (half cone only).
                if (colliderType == ExtendedColliders3D.ColliderType.ConeHalf) {
                    EditorGUI.BeginChangeCheck();
                    bool coneHalfCapFlatEnd = EditorGUILayout.Toggle(new GUIContent("Cap Flat End", "Whether to cap the flat side of the half cone."),
                            extendedColliders3D.properties.coneHalfCapFlatEnd);
                    if (EditorGUI.EndChangeCheck()) {
                        beginUndo("Change Extended Colliders Half Cone Cap Flat End Flag");
                        extendedColliders3D.properties.coneHalfCapFlatEnd = coneHalfCapFlatEnd;
                        endUndo(true);
                    }
                }
            }

            //Cube faces.
            if (colliderType == ExtendedColliders3D.ColliderType.Cube) {
                for (int i = 0; i < 6; i++) {
                    string faceName = new string[] { "Top", "Bottom", "Left", "Right", "Forward", "Back" }[i];
                    EditorGUI.BeginChangeCheck();
                    bool cubeFace = EditorGUILayout.Toggle(new GUIContent(faceName + " Face", "Whether the cube should have its " + faceName.ToLower() + " face."),
                            i == 0 ? extendedColliders3D.properties.cubeTopFace : (i == 1 ? extendedColliders3D.properties.cubeBottomFace :
                            (i == 2 ? extendedColliders3D.properties.cubeLeftFace : (i == 3 ? extendedColliders3D.properties.cubeRightFace :
                            (i == 4 ? extendedColliders3D.properties.cubeForwardFace : extendedColliders3D.properties.cubeBackFace)))));
                    if (EditorGUI.EndChangeCheck()) {
                        beginUndo("Change Extended Colliders Cube " + faceName + " Face");
                        if (i == 0)
                            extendedColliders3D.properties.cubeTopFace = cubeFace;
                        else if (i == 1)
                            extendedColliders3D.properties.cubeBottomFace = cubeFace;
                        else if (i == 2)
                            extendedColliders3D.properties.cubeLeftFace = cubeFace;
                        else if (i == 3)
                            extendedColliders3D.properties.cubeRightFace = cubeFace;
                        else if (i == 4)
                            extendedColliders3D.properties.cubeForwardFace = cubeFace;
                        else
                            extendedColliders3D.properties.cubeBackFace = cubeFace;
                        endUndo(true);
                    }
                }
            }

            //(Half) cylinder-specific settings.
            if (colliderType == ExtendedColliders3D.ColliderType.Cylinder || colliderType == ExtendedColliders3D.ColliderType.CylinderHalf) {

                //Cylinder faces.
                EditorGUI.BeginChangeCheck();
                int cylinderFaces = Math.Max(Math.Min(EditorGUILayout.IntField(new GUIContent("Faces",
                        "The number of faces the cylinder will have. More faces mean a smoother surface."), extendedColliders3D.properties.cylinderFaces), 256),
                        3);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cylinder Faces");
                    extendedColliders3D.properties.cylinderFaces = cylinderFaces;
                    endUndo(true);
                }

                //Cap top.
                EditorGUI.BeginChangeCheck();
                bool cylinderCapTop = EditorGUILayout.Toggle(new GUIContent("Cap Top", "Whether to add a cap onto the top of the cylinder."),
                        extendedColliders3D.properties.cylinderCapTop);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cylinder Cap Top Flag");
                    extendedColliders3D.properties.cylinderCapTop = cylinderCapTop;
                    endUndo(true);
                }

                //Cap bottom.
                EditorGUI.BeginChangeCheck();
                bool cylinderCapBottom = EditorGUILayout.Toggle(new GUIContent("Cap Bottom", "Whether to add a cap onto the bottom of the cylinder."),
                        extendedColliders3D.properties.cylinderCapBottom);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cylinder Cap Bottom Flag");
                    extendedColliders3D.properties.cylinderCapBottom = cylinderCapBottom;
                    endUndo(true);
                }

                //Cap flat end (half cylinder only).
                if (colliderType == ExtendedColliders3D.ColliderType.CylinderHalf) {
                    EditorGUI.BeginChangeCheck();
                    bool cylinderHalfCapFlatEnd = EditorGUILayout.Toggle(new GUIContent("Cap Flat End", "Whether to cap the flat side of the half cylinder."),
                            extendedColliders3D.properties.cylinderHalfCapFlatEnd);
                    if (EditorGUI.EndChangeCheck()) {
                        beginUndo("Change Extended Colliders Half Cylinder Cap Flat End Flag");
                        extendedColliders3D.properties.cylinderHalfCapFlatEnd = cylinderHalfCapFlatEnd;
                        endUndo(true);
                    }
                }

                //Taper top.
                EditorGUI.BeginChangeCheck();
                Vector2 cylinderTaperTop = EditorGUILayout.Vector2Field(new GUIContent("Taper Top", "The amount to \"taper\" the top of the cylinder. Tapering " +
                        "stretches the cylinder to allow the top and bottom to have different radii, to create, for example, a funnel shape."),
                        extendedColliders3D.properties.cylinderTaperTop);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cylinder Taper Top");
                    extendedColliders3D.properties.cylinderTaperTop = cylinderTaperTop;
                    endUndo(true);
                }

                //Taper bottom.
                EditorGUI.BeginChangeCheck();
                Vector2 cylinderTaperBottom = EditorGUILayout.Vector2Field(new GUIContent("Taper Top", "The amount to \"taper\" the top of the cylinder. " +
                        "Tapering stretches the cylinder to allow the top and bottom to have different radii, to create, for example, a funnel shape."),
                        extendedColliders3D.properties.cylinderTaperBottom);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Cylinder Taper Bottom");
                    extendedColliders3D.properties.cylinderTaperBottom = cylinderTaperBottom;
                    endUndo(true);
                }
            }

            //Quad two-sided.
            if (colliderType == ExtendedColliders3D.ColliderType.Quad) {
                EditorGUI.BeginChangeCheck();
                bool quadTwoSided = EditorGUILayout.Toggle(new GUIContent("Two-sided", "Whether the quad should allow collisions from both sides."),
                        extendedColliders3D.properties.quadTwoSided);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Quad Two-Sided Flag");
                    extendedColliders3D.properties.quadTwoSided = quadTwoSided;
                    endUndo(true);
                }
            }

            //Triangle two-sided.
            if (colliderType == ExtendedColliders3D.ColliderType.Triangle) {
                EditorGUI.BeginChangeCheck();
                bool triangleTwoSided = EditorGUILayout.Toggle(new GUIContent("Two-sided", "Whether the triangle should allow collisions from both sides."),
                        extendedColliders3D.properties.triangleTwoSided);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Triangle Two-Sided Flag");
                    extendedColliders3D.properties.triangleTwoSided = triangleTwoSided;
                    endUndo(true);
                }
            }

            //Sphere-specific settings.
            if (colliderType == ExtendedColliders3D.ColliderType.Sphere) {

                //Stacks.
                EditorGUI.BeginChangeCheck();
                int sphereStacks = Math.Max(Math.Min(EditorGUILayout.IntField(new GUIContent("Stacks",
                        "The number of distinct vertical bands on the sphere. Increase this for a more detailed collider."),
                        extendedColliders3D.properties.sphereStacks), 64), 3);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Sphere Stacks");
                    extendedColliders3D.properties.sphereStacks = sphereStacks;
                    endUndo(true);
                }

                //Slices.
                EditorGUI.BeginChangeCheck();
                int sphereSlices = Math.Max(Math.Min(EditorGUILayout.IntField(new GUIContent("Slices",
                        "The number of slices to separate the sphere into. Increase this for a more detailed collider."),
                        extendedColliders3D.properties.sphereSlices), 64), 3);
                if (EditorGUI.EndChangeCheck()) {
                    beginUndo("Change Extended Colliders Sphere Slices");
                    extendedColliders3D.properties.sphereSlices = sphereSlices;
                    endUndo(true);
                }
            }

            //Editor settings header.
            EditorGUILayout.GetControlRect();
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

            //Colour.
            EditorGUI.BeginChangeCheck();
            Color colour = EditorGUILayout.ColorField(new GUIContent("Colour", "The colour to draw the collider gizmo in the editor."),
                    extendedColliders3D.properties.colour);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Editor Colour");
                extendedColliders3D.properties.colour = colour;
                endUndo(true);
            }

            //Gizmo type.
            EditorGUI.BeginChangeCheck();
            ExtendedColliders3D.GizmoType gizmoType = (ExtendedColliders3D.GizmoType) EditorGUILayout.EnumPopup(new GUIContent("Gizmo Type",
                    "Which gizmos to show in the scene view to manipulate the Extended Colliders 3D properties - position, rotation or scale."),
                    extendedColliders3D.properties.gizmoType);
            if (EditorGUI.EndChangeCheck()) {
                beginUndo("Change Extended Colliders Editor Gizmo Type");
                extendedColliders3D.properties.gizmoType = gizmoType;
                basePositionRotationOrScaleDirty = true;
                endUndo(true);
            }

            //Version details.
            GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
            versionStyle.fontStyle = FontStyle.Bold;
            versionStyle.normal.textColor = new Color(0, 0.4f, 0.8f);
            versionStyle.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.GetControlRect();
            rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(new Rect(rect.xMax - 100, rect.yMin, 100, rect.height), "Version 1.0.5", versionStyle)) {
                ExtendedColliders3DVersionChanges versionChangesEditorWindow = EditorWindow.GetWindow<ExtendedColliders3DVersionChanges>();
                versionChangesEditorWindow.minSize = new Vector2(800, 600);
                versionChangesEditorWindow.titleContent = new GUIContent("Extended Colliders 3D - Version Changes");
            }
        }

        //Draw the scene view, including any gizmos.
        protected virtual void OnSceneGUI() {

            //Get the Extended Colliders 3D component.
            ExtendedColliders3D extendedColliders3D = (ExtendedColliders3D) target;

            //Don't bother drawing anything to the scene view if gizmos are turned off.
            if (extendedColliders3D.properties.gizmoType != ExtendedColliders3D.GizmoType.None) {

                //Store the old handle colour so it can be restored at the end of this method.
                Color oldHandlesColour = Handles.color;

                //Get the handle size. Make it twice as big as the regular Unity Editor handles to distinguish.
                float handleSize = HandleUtility.GetHandleSize(extendedColliders3D.transform.position) * 2;

                //Set the handle position and rotation.
                Vector3 position = extendedColliders3D.properties.centre;
                Quaternion rotationWithColliderSettings = Quaternion.Euler(extendedColliders3D.properties.rotation);
                Quaternion rotationWithoutColliderSettings = Quaternion.identity;
                Transform currentTransform = extendedColliders3D.transform;
                while (currentTransform != null) {
                    position.x *= currentTransform.localScale.x;
                    position.y *= currentTransform.localScale.y;
                    position.z *= currentTransform.localScale.z;
                    position = currentTransform.localRotation * position;
                    position += currentTransform.localPosition;
                    rotationWithColliderSettings = currentTransform.localRotation * rotationWithColliderSettings;
                    rotationWithoutColliderSettings = currentTransform.localRotation * rotationWithoutColliderSettings;
                    currentTransform = currentTransform.parent;
                }

                //Handle position gizmos.
                if (extendedColliders3D.properties.gizmoType == ExtendedColliders3D.GizmoType.Position) {
                    bool positionHandleDragged = false;
                    if (!basePositionRotationOrScaleDirty)
                        for (int i = 0; i < 3; i++) {
                            Handles.color = i == 0 ? Handles.xAxisColor : (i == 1 ? Handles.yAxisColor : Handles.zAxisColor);
                            Vector3 direction = i == 0 ? Vector3.right : (i == 1 ? Vector3.up : Vector3.forward);
                            Vector3 positionXYZ = Handles.Slider(
                                baseGlobalPosition,
                                rotationWithoutColliderSettings * direction,
                                handleSize,
#if UNITY_5_5_OR_NEWER
                                Handles.ArrowHandleCap,
#else
                                Handles.ArrowCap,
#endif
                                0);
                            if (Vector3.Distance(positionXYZ, baseGlobalPosition) > 0.0001f) {
                                beginUndo("Change Extended Colliders Centre Point");
                                Vector3 movement = basePositionRotationOrScale + (rotationWithoutColliderSettings * (positionXYZ - baseGlobalPosition));
                                if (i == 0)
                                    extendedColliders3D.properties.centre.x = movement.x;
                                else if (i == 1)
                                    extendedColliders3D.properties.centre.y = movement.y;
                                else
                                    extendedColliders3D.properties.centre.z = movement.z;
                                endUndo(false);
                                positionHandleDragged = true;
                            }

                        }
                    if (basePositionRotationOrScaleDirty || (!positionHandleDragged && Event.current.type == EventType.Used)) {
                        baseGlobalPosition = position;
                        basePositionRotationOrScale = extendedColliders3D.properties.centre;
                        basePositionRotationOrScaleDirty = false;
                        Repaint();
                    }
                }

                //Handle rotation gizmos.
                else if (extendedColliders3D.properties.gizmoType == ExtendedColliders3D.GizmoType.Rotation) {
                    bool rotationHandleDragged = false;
                    if (!basePositionRotationOrScaleDirty) {
                        Vector3 eulerAngles = (Quaternion.Inverse(rotationWithoutColliderSettings) * Handles.RotationHandle(rotationWithoutColliderSettings,
                                position)).eulerAngles;
                        if (eulerAngles.magnitude > 0.0001f) {
                            beginUndo("Change Extended Colliders Rotation");
                            extendedColliders3D.properties.rotation.x = basePositionRotationOrScale.x + eulerAngles.x;
                            extendedColliders3D.properties.rotation.y = basePositionRotationOrScale.y + eulerAngles.y;
                            extendedColliders3D.properties.rotation.z = basePositionRotationOrScale.z + eulerAngles.z;
                            endUndo(false);
                            rotationHandleDragged = true;
                        }
                    }
                    if (basePositionRotationOrScaleDirty || (!rotationHandleDragged && Event.current.type == EventType.Used)) {
                        basePositionRotationOrScale = extendedColliders3D.properties.rotation;
                        basePositionRotationOrScaleDirty = false;
                        Repaint();
                    }
                }

                //Handle scale gizmos.
                else if (extendedColliders3D.properties.gizmoType == ExtendedColliders3D.GizmoType.Scale) {
                    bool scaleHandleDragged = false;
                    if (!basePositionRotationOrScaleDirty)
                        for (int i = 0; i < 3; i++) {
                            if (i == 1 && (extendedColliders3D.properties.colliderType == ExtendedColliders3D.ColliderType.Circle ||
                                    extendedColliders3D.properties.colliderType == ExtendedColliders3D.ColliderType.CircleHalf ||
                                    extendedColliders3D.properties.colliderType == ExtendedColliders3D.ColliderType.Triangle ||
                                    extendedColliders3D.properties.colliderType == ExtendedColliders3D.ColliderType.Quad))
                                continue;
                            Handles.color = i == 0 ? Handles.xAxisColor : (i == 1 ? Handles.yAxisColor : Handles.zAxisColor);
                            float scaleXYZ = Handles.ScaleSlider(
                                1,
                                position,
                                rotationWithColliderSettings * (i == 0 ? Vector3.right : (i == 1 ? Vector3.up : Vector3.forward)),
                                rotationWithColliderSettings,
                                handleSize,
                                0);

                            if (Mathf.Abs(scaleXYZ - 1) > 0.0001f) {
                                beginUndo("Change Extended Colliders Scale");
                                float scale = (i == 0 ? basePositionRotationOrScale.x : (i == 1 ? basePositionRotationOrScale.y : basePositionRotationOrScale.z)) +
                                        (scaleXYZ - 1);
                                if (scale > 0)
                                    scale = Mathf.Max(scale, 0.1f);
                                else
                                    scale = Mathf.Min(scale, -0.1f);
                                if (i == 0)
                                    extendedColliders3D.properties.size.x = scale;
                                else if (i == 1)
                                    extendedColliders3D.properties.size.y = scale;
                                else
                                    extendedColliders3D.properties.size.z = scale;
                                endUndo(false);
                                scaleHandleDragged = true;
                            }
                        }
                    if (basePositionRotationOrScaleDirty || (!scaleHandleDragged && Event.current.type == EventType.Used)) {
                        basePositionRotationOrScale = extendedColliders3D.properties.size;
                        basePositionRotationOrScaleDirty = false;
                        Repaint();
                    }
                }

                //Restore the old handles colour.
                Handles.color = oldHandlesColour;
            }
        }

        //Begin an undo.
        void beginUndo(string eventName) {
            if (Undo.undoRedoPerformed == null)
                Undo.undoRedoPerformed += delegate { lastUndoEvent = ""; };
            Undo.RecordObject(target, eventName);
            if (eventName != lastUndoEvent) {
                undoGroup = Undo.GetCurrentGroup();
                lastUndoEvent = eventName;
            }
        }

        //End an undo.
        void endUndo(bool forceNewGroup) {
            EditorUtility.SetDirty(target);
            if (forceNewGroup)
                lastUndoEvent = "";
            else
                Undo.CollapseUndoOperations(undoGroup);
        }
    }
}