using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;
        Vector2 scrollPosition;

        const float canvasSize = 4000f;
        const float backGroundSize = 50f;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue)
            {
                ShowEditorWindow();
            }            
            return false;
        }

        void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChange;
        }

        void OnSelectionChange()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backGroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize/backGroundSize, canvasSize/backGroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backGroundTex, texCoords);

;               foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }              
        }

        

        void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.Rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else //node is null because clicked on empty space
                {
                    Selection.activeObject = selectedDialogue;
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }        

        void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;

            if (node.IsPlayerSpeaking)
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.Rect, style);

            node.SetText(EditorGUILayout.TextField(node.Text));

            GUILayout.BeginHorizontal();
            Color defaultGUIColor = GUI.backgroundColor; // Saves the default color

            GUI.backgroundColor = Color.green; // Set the color for Delete-button
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            DrawLinkButtons(node);

            GUI.backgroundColor = Color.red; // Set the color for the Add-            
            if (GUILayout.Button("-"))
            {
                deletingNode = node;
            }
            GUI.backgroundColor = defaultGUIColor;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        void DrawLinkButtons(DialogueNode node)
        {
            GUI.backgroundColor = Color.magenta;
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.Children.Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.blue, null, 4f);
            }
        }

        DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.Rect.Contains(point)) foundNode = node;           
            }
            return foundNode;
        }
    }
}