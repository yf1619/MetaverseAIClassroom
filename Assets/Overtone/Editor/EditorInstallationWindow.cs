using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LeastSquares.Overtone
{

    /// <summary>
    /// A window that provides a guide to get started with Undertone.
    /// </summary>
    public class OvertoneGettingStartedWindow : EditorWindow
    {
        private static bool showOnStart = true;
        private const string showOnStartPrefKey = "OvertoneShowOnStart";
        private Vector2 scrollPosition;

        /// <summary>
        /// Adds a menu item to open the Undertone Getting Started window.
        /// </summary>
        [MenuItem("Window/Overtone/Getting Started")]
        public static void ShowWindow()
        {
            showOnStart = EditorPrefs.GetBool(showOnStartPrefKey, true);
            if (showOnStart)
                GetWindow<OvertoneGettingStartedWindow>("Overtone Getting Started");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginVertical("Box");
            {
                RenderWelcomeLabel();
                RenderSpeechEngineSection();
                RenderModelTypesSection();
                RenderDemoScenesSection();
                RenderDocsAndLinks();
                RenderStartupToggle();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Renders the welcome label at the top of the window.
        /// </summary>
        private void RenderWelcomeLabel()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Getting Started with Overtone", EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Renders the section for adding a speech engine and downloading a model.
        /// </summary>
        private void RenderSpeechEngineSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 1: Add a TTSEngine", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "To use Overtone, you need to add a TTS engine into your game. Attach the script to a game object",
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 2: Add a TTSVoice and download a model", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "You need to add a TTS voice into your game and download a model. The included 'libritts.en' model can be used to start. The TTSVoice will represent the voice used to express the text",
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 3: Add a TTSPlayer", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "The TTSPlayer combines a TTSVoice and a TTSEngine and outputs the speech in an AudioSource",
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Renders the section for understanding different model types.
        /// </summary>
        private void RenderModelTypesSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 4: Understand different model types", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "There are different model types for each language. Each model has a quality (X-LOW, LOW, MEDIUM, HIGH) and can contain multiple voices. For example the libritts english contains 900+ different voices.",
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Renders the section for checking out the demo scenes.
        /// </summary>
        private void RenderDemoScenesSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 5: Check out the demo scenes", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("The following demo scenes are available for you to try:",
                    MessageType.Info);
                for (int i = 0; i < demoSceneNames.Length; i++)
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button(demoSceneNames[i].Replace("Assets/Overtone/", ""), GUILayout.Height(25), GUILayout.ExpandWidth(true)))
                    {
                        EditorSceneManager.OpenScene(demoSceneNames[i]);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Renders the section for checking out the documentation and links.
        /// </summary>
        private void RenderDocsAndLinks()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Step 4: Check out the documentation & more", EditorStyles.boldLabel);
                if (GUILayout.Button("Open Documentation"))
                    Application.OpenURL("https://leastsquares.io/docs/unity/overtone");
                if (GUILayout.Button("Join Discord Community"))
                    Application.OpenURL("https://discord.gg/DZpBsTYNPD");
                if (GUILayout.Button("Check Other Assets"))
                    Application.OpenURL("https://assetstore.unity.com/publishers/39777");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Renders the toggle for showing the window on startup.
        /// </summary>
        private void RenderStartupToggle()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Show this window on startup", GUILayout.Width(400));
                showOnStart = EditorGUILayout.Toggle(showOnStart);
                EditorPrefs.SetBool(showOnStartPrefKey, showOnStart);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private static readonly string[] demoSceneNames =
        {
            "Assets/Overtone/Demos/Text to speech.unity",
        };
    }
}