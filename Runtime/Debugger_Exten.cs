#define ENABLE_ALL_COMMANDS
#define ENABLE_PLAYER_PREFS_COMMANDS
#define ENABLE_STANDARD_COMMANDS
#define ENABLE_EDITOR_COMMANDS
#define ENABLE_HTTP_COMMANDS
#define ENABLE_SYSTEM_COMMANDS

using uDebugger.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_HTTP_COMMANDS
using System.Net;
#endif

#if ENABLE_SYSTEM_COMMANDS
using System.Diagnostics;
using System.IO;
using System.Linq;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Compilation;
#endif

using UnityDebug = UnityEngine.Debug;

namespace uDebugger
{
#if ENABLE_ALL_COMMANDS
    public static partial class Debugger
    {
        public static event System.Action OnClearConsoleEvent;

#if ENABLE_STANDARD_COMMANDS
        /// <summary>
        /// 
        /// </summary>
        [DebugMethod(alias: "showInExplorer")]
        private static void ShowInExplorer()
        {
            var objPath = string.Empty;

#if UNITY_EDITOR
            if (Selection.activeObject != null)
                objPath = AssetDatabase.GetAssetPath(Selection.activeObject);
#endif

            if (string.IsNullOrEmpty(objPath))
            {
#if UNITY_EDITOR
                objPath = EditorSceneManager.GetActiveScene().path;
#else
                objPath = SceneManager.GetActiveScene().path;
#endif
            }

            var dataPath = Application.dataPath.Replace("Assets", string.Empty);
            var urlPath = System.IO.Path.Combine(dataPath, objPath);

            urlPath = urlPath.Replace(@"/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + urlPath);
        }

        /// <summary>
        /// 
        /// </summary>
        [DebugMethod(alias: "openFilePath")]
        private static void OpenFilePath(string relativePath)
        {
            var dataPath = Application.dataPath.Replace("Assets", string.Empty);

#if UNITY_EDITOR
            var urlPath = System.IO.Path.Combine(dataPath, relativePath);
#else
            var urlPath = Path.Combine(dataPath, relativePath);
#endif

            Application.OpenURL($"file:///{urlPath}");
        }

        /// <summary>
        /// 
        /// </summary>
        [DebugMethod(alias: "showFilePath")]
        private static void ShowFilePath(string relativePath)
        {
            var dataPath = Application.dataPath.Replace("Assets", string.Empty);
            var urlPath = System.IO.Path.Combine(dataPath, relativePath);

            urlPath = urlPath.Replace(@"/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + urlPath);
        }

        [DebugMethod(alias: "clear", altAlias: "cls")]
        private static void ClearConsole()
        {
#if UNITY_EDITOR
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#else
            UnityDebug.ClearDeveloperConsole();
#endif

            OnClearConsoleEvent?.Invoke();
        }

        [DebugMethod(alias: "application.getDataPath")]
        private static string Application_Get_DataPath() => Application.dataPath;

        [DebugMethod(alias: "application.getStreamingAssetsPath")]
        private static string Application_Get_StreamingAssetsPath() => Application.streamingAssetsPath;

        [DebugMethod(alias: "application.getPersistentDataPath")]
        private static string Application_Get_PersistentDataPath() => Application.persistentDataPath;

        [DebugMethod(alias: "application.getConsoleLogPath")]
        private static string Application_Get_ConsoleLogPath() => Application.consoleLogPath;

        [DebugMethod(alias: "application.getTemporaryCachePath")]
        private static string Application_Get_TemporaryCachePath() => Application.temporaryCachePath;

        [DebugMethod(alias: "application.getRunInBackground")]
        private static bool Application_Get_RunInBackground() => Application.runInBackground;

        [DebugMethod(alias: "application.setRunInBackground")]
        private static void Application_Set_RunInBackground(bool value) => Application.runInBackground = value;

        [DebugMethod(alias: "application.getPlatform")]
        private static RuntimePlatform Application_Get_Platform() => Application.platform;

        [DebugMethod(alias: "application.getSystemLanguage")]
        private static SystemLanguage Application_Get_SystemLanguage() => Application.systemLanguage;

        [DebugMethod(alias: "application.getTargetFrameRate")]
        private static int Application_Get_TargetFrameRate() => Application.targetFrameRate;

        [DebugMethod(alias: "application.setTargetFrameRate")]
        private static void Application_Set_TargetFrameRate(int value) => Application.targetFrameRate = value;

        [DebugMethod(alias: "reloadScene", description: "Reload current scene")]
        private static void ReloadScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        [DebugMethod(alias: "goto.help", altAlias: "help")]
        private static void GotToHelp()
        {
            var builder = new System.Text.StringBuilder();

            //foreach (var command in DebuggerCollection.MethodCommands)
            //    builder.Append("\n" + $"{command.Address}: {command.Description}");

            UnityDebug.Log(builder.ToString());
        }

        [DebugMethod(alias: "goto.fullscreen", altAlias: "fullscreen")]
        private static void GoToFullscreen()
        {
#if UNITY_EDITOR
            //var view = GetGameView();
            //view.maximized = !view.maximized;

            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#else
            Screen.fullScreen = !Screen.fullScreen;
#endif
        }

        [DebugMethod("goto.scene")]
        private static void GoToScene(string sceneName)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                EditorSceneManager.LoadScene(sceneName);
            else
            {
                //var scene = EditorFindSceneByName(sceneName);
                //EditorSceneManager.OpenScene(scene.path);
            }
#else
            SceneManager.LoadScene(sceneName);
#endif
        }

#if UNITY_EDITOR
        private static EditorBuildSettingsScene EditorFindSceneByName(string name)
        {
            var scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                if (scene.path.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                    return scene;
            }

            return default;
        }
#endif

        [DebugMethod("goto.scene")]
        private static void GoToScene(string sceneName, LoadSceneMode mode)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                EditorSceneManager.LoadScene(sceneName, mode);
            else
            {
                var openMode = OpenSceneMode.Single;

                switch (mode)
                {
                    case LoadSceneMode.Single:
                        openMode = OpenSceneMode.Single;
                        break;
                    case LoadSceneMode.Additive:
                        openMode = OpenSceneMode.Additive;
                        break;
                }

                var scene = EditorFindSceneByName(sceneName);
                EditorSceneManager.OpenScene(scene.path, openMode);
            }
#else
            SceneManager.LoadScene(sceneName, mode);
#endif
        }

        [DebugMethod("goto.scene")]
        private static void GoToScene(int sceneBuildIndex)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                EditorSceneManager.LoadScene(sceneBuildIndex);
            else
                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex));
#else
            SceneManager.LoadScene(sceneBuildIndex);
#endif
        }

        [DebugMethod("goto.scene")]
        private static void GoToScene(int sceneBuildIndex, LoadSceneMode mode)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                EditorSceneManager.LoadScene(sceneBuildIndex, mode);
            else
            {
                var openMode = OpenSceneMode.Single;

                switch (mode)
                {
                    case LoadSceneMode.Single:
                        openMode = OpenSceneMode.Single;
                        break;
                    case LoadSceneMode.Additive:
                        openMode = OpenSceneMode.Additive;
                        break;
                }

                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex), openMode);
            }
#else
            SceneManager.LoadScene(sceneBuildIndex, mode);
#endif
        }

        [DebugMethod("goto.quit")]
        private static void GoToQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(exitCode: 0);
#endif
        }

        [DebugMethod("screen.getFullscreen", "Get value of fullscreen")]
        private static bool Screen_Get_Fullscreen() => Screen.fullScreen;

        [DebugMethod("screen.setFullscreen", "Set value of fullscreen")]
        private static void Screen_Set_Fullscreen(bool value) => Screen.fullScreen = value;

        [DebugMethod("screen.setResolution", "Set value of Resolution")]
        private static void Screen_Set_Resolution(int width, int height) => Screen.SetResolution(width, height, Screen.fullScreen, Screen.currentResolution.refreshRate);

        [DebugMethod("screen.getResolution", "Get value of Resolution")]
        private static Resolution Screen_Get_Resolution() => Screen.currentResolution;

        [DebugMethod("mute", "Mute volume")]
        private static void Mute() => AudioListener.volume = 0f;

        [DebugMethod("unmute", "Unmute volume")]
        private static void Unmute() => AudioListener.volume = 1f;

        [DebugMethod("audioListener.setVolume", "Set volume to specific value")]
        private static void AudioListener_Set_Volume(float value) => AudioListener.volume = Mathf.Clamp01(value);

        [DebugMethod("audioListener.getVolume", "Gets current volume")]
        private static float AudioListener_Get_Volume() => AudioListener.volume;

        [DebugMethod("spawn.rigidCube", "Spawns a cube at location")]
        private static void SpawnRigidCube(Vector3 pos) => SpawnRigidPrimitive(PrimitiveType.Cube, pos);

        [DebugMethod("spawn.rigidSphere", "Spawns a sphere at location")]
        private static void SpawnRigidSphere(Vector3 pos) => SpawnRigidPrimitive(PrimitiveType.Sphere, pos);

        [DebugMethod("spawn.cube", "Spawns a cube at location")]
        private static void SpawnCube(Vector3 pos) => SpawnPrimitive(PrimitiveType.Cube, pos);

        [DebugMethod("spawn.sphere", "Spawns a sphere at location")]
        private static void SpawnSphere(Vector3 pos) => SpawnPrimitive(PrimitiveType.Sphere, pos);

        private static void SpawnPrimitive(PrimitiveType type, Vector3 pos)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.transform.position = pos;
        }

        private static void SpawnRigidPrimitive(PrimitiveType type, Vector3 pos)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.transform.position = pos;
            var body = obj.AddComponent<Rigidbody>();
            body.mass = 1f;
            body.useGravity = true;
        }

#if ENABLE_PLAYER_PREFS_COMMANDS
        [DebugMethod("playerPrefs.setString", description: "Get information from the application")]
        private static void PrefsSetString(string key, string value) => PlayerPrefs.SetString(key, value);

        [DebugMethod("playerPrefs.setInt", description: "Get information from the application")]
        private static void PrefsSetInt(string key, int value) => PlayerPrefs.SetInt(key, value);

        [DebugMethod("playerPrefs.setFloat", description: "Get information from the application")]
        private static void PrefsSetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);

        [DebugMethod("playerPrefs.save", description: "Get information from the application")]
        private static void SavePrefs(string key, float value) => PlayerPrefs.Save();

        [DebugMethod("playerPrefs.deleteKey", description: "Get information from the application")]
        private static void PrefsDeleteKey(string key) => PlayerPrefs.DeleteKey(key);

        [DebugMethod("playerPrefs.deleteAll", description: "Get information from the application")]
        private static void PrefsDeleteAll() => PlayerPrefs.DeleteAll();

        [DebugMethod("playerPrefs.getFloat", description: "Get information from the application")]
        private static void PrefsGetFloat(string key) => PlayerPrefs.GetFloat(key);

        [DebugMethod("playerPrefs.getInt", description: "Get information from the application")]
        private static void PrefsGetInt(string key) => PlayerPrefs.GetInt(key);

        [DebugMethod("playerPrefs.getString", description: "Get information from the application")]
        private static void PrefsGetString(string key) => PlayerPrefs.GetString(key);

        [DebugMethod("playerPrefs.hasKey", description: "Get information from the application")]
        private static bool PrefsHasKey(string key) => PlayerPrefs.HasKey(key);
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        [DebugMethod(alias: "screenshot")]
        private static void Screenshot(string fileName) => ScreenCapture.CaptureScreenshot(fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod("application.getInfo", description: "Get information from the application")]
        private static string Application_Get_Info() => $"{Application.productName} by {Application.companyName} (Version: {Application.version})";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        [DebugMethod(alias: "time.setTimeScale")]
        private static void Time_Set_TimeScale(float scale) => Time.timeScale = Mathf.Max(scale, 0f);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "time.getTimeScale")]
        private static float Time_Get_TimeScale() => Time.timeScale;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod(alias: "physics.setGravity")]
        private static void Physics_Set_Gravity(Vector3 gravity) => Physics.gravity = gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "physics.getGravity")]
        private static Vector3 Physics_Get_Gravity() => Physics.gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod(alias: "physics.setGravity2D")]
        private static void Physics_Set_Gravity2D(Vector2 gravity) => Physics2D.gravity = gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "physics.getGravity2D")]
        private static Vector2 Physics_Get_Gravity2D() => Physics2D.gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod("physics.setClothGravity", description: "Sets the cloth gravity")]
        private static void Physics_Set_ClothGravity(Vector3 gravity) => Physics.clothGravity = gravity;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod("physics.getClothGravity")]
        private static Vector3 Physics_Get_ClothGravity() => Physics.clothGravity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod("physics.setSleepThreshold", description: "Sets the sleep threshold")]
        private static void Physics_Set_SleepThreshold(float sleepThreshold) => Physics.sleepThreshold = sleepThreshold;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod("physics.getSleepThreshold", description: "Gets the sleep threshold")]
        private static float Physics_Get_SleepThreshold() => Physics.sleepThreshold;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod("physics.getDefaultMaxAngularSpeed", description: "Gets the Default Max Angular Speed (default value is 50)")]
        private static float Physics_Get_DefaultMaxAngularSpeed() => Physics.defaultMaxAngularSpeed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gravity"></param>
        [DebugMethod("physics.setDefaultMaxAngularSpeed", description: "Sets the Default Max Angular Speed (default value is 50)")]
        private static void Physics_Set_DefaultMaxAngularSpeed(float maxAngularSpeed) => Physics.defaultMaxAngularSpeed = maxAngularSpeed;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "break")]
        private static void Break() => UnityDebug.Break();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        [DebugMethod(alias: "log", altAlias: "print")]
        private static void Log(string message) => UnityDebug.Log(message);

#endif

#if UNITY_EDITOR
        private static EditorWindow GetGameView()
        {
            var assembly = typeof(EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            return EditorWindow.GetWindow(type);
        }

        private static EditorWindow GetSceneView()
        {
            var assembly = typeof(EditorWindow).Assembly;
            var type = assembly.GetType("UnityEditor.SceneView");
            return EditorWindow.GetWindow(type);
        }

        private static EditorWindow GetEditorSettingsView()
        {
            var type = System.Type.GetType("UnityEditor.PlayerSettings,UnityEditor");
            return EditorWindow.GetWindow(type);
        }

        private static EditorWindow GetBuildSettingsView()
        {
            var type = System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor");
            return EditorWindow.GetWindow(type);
        }
#endif

#if UNITY_EDITOR && ENABLE_EDITOR_COMMANDS

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.step")]
        private static void EditorStep() => EditorApplication.Step();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.focus.SceneView")]
        private static void FocusSceneView()
        {
            var view = GetSceneView();
            view.Focus();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.focus.buildSettings")]
        private static void FocusBuildSettings()
        {
            var view = GetBuildSettingsView();
            view.Focus();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.focus.projectSettings")]
        private static void FocusProjectSettings()
        {
            EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.focus.gameView")]
        private static void FocusMainView()
        {
            var view = GetGameView();
            view.Focus();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.exit")]
        private static void EditorExit()
        {
            if (!EditorUtility.DisplayDialog(title: "Exiting from the editor", message: "Are you sure you want to exit from the editor?", ok: "Yes", cancel: "No"))
                return;

            EditorApplication.Exit(returnValue: 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.saveScene")]
        private static void EditorSaveScene() => EditorSceneManager.SaveOpenScenes();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.play")]
        private static void EditorPlay() => EditorApplication.isPlaying = true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.stop")]
        private static void EditorStop() => EditorApplication.isPlaying = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.pause")]
        private static void EditorPause() => EditorApplication.isPaused = true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.resume")]
        private static void EditorResume() => EditorApplication.isPaused = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "editor.beep")]
        private static void EditorBeep() => EditorApplication.Beep();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        [DebugMethod(alias: "editor.openScene")]
        private static void EditorOpenScene(int sceneBuildIndex)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);
            EditorSceneManager.OpenScene(scenePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="mode"></param>
        [DebugMethod(alias: "editor.openScene")]
        private static void EditorOpenScene(string scenePath, OpenSceneMode mode) => EditorSceneManager.OpenScene(scenePath, mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        /// <param name="mode"></param>
        [DebugMethod(alias: "editor.openScene")]
        private static void EditorOpenScene(int sceneBuildIndex, OpenSceneMode mode)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);
            EditorSceneManager.OpenScene(scenePath, mode);
        }

        /// <summary>
        /// 
        /// </summary>
        [DebugMethod(alias: "editor.export")]
        private static void EditorExport()
        {
            if (!EditorUtility.DisplayDialog(title: "Exporting your application", message: "Are you sure you want to export/build your application?", ok: "Yes", cancel: "No"))
                return;

            var destinationPath = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

            var scenes = EditorBuildSettings.scenes;
            var levels = scenes.Select(x => x.path).ToArray();

            var target = EditorUserBuildSettings.activeBuildTarget;
            var filePath = System.IO.Path.Combine(destinationPath, $"{Application.productName}.exe");

            BuildPipeline.BuildPlayer(levels, filePath, target, BuildOptions.None);

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = filePath;
            proc.Start();
        }

        [DebugMethod("editor.debugMode")]
        private static void Editor_Switch_DebugMode()
        {
            if (CompilationPipeline.codeOptimization == CodeOptimization.Debug)
                return;

            if (!EditorUtility.DisplayDialog(title: $"Switching from release to debug mode", message: "Are you sure you want to switch?", ok: "Yes", cancel: "No"))
                return;

            CompilationPipeline.codeOptimization = CodeOptimization.Debug;
        }

        [DebugMethod("editor.releaseMode")]
        private static void Editor_Switch_ReleaseMode()
        {
            if (CompilationPipeline.codeOptimization == CodeOptimization.Release)
                return;

            if (!EditorUtility.DisplayDialog(title: $"Switching from debug to release mode", message: "Are you sure you want to switch?", ok: "Yes", cancel: "No"))
                return;

            CompilationPipeline.codeOptimization = CodeOptimization.Release;
        }
#endif

#if ENABLE_HTTP_COMMANDS
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "http.get")]
        private static void HttpGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            UnityDebug.Log(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "http.put")]
        private static void HttpPut(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            UnityDebug.Log(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "http.post")]
        private static void HttpPost(string url, string bodyText)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var data = System.Text.Encoding.ASCII.GetBytes(bodyText);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            UnityDebug.Log(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "http.delete")]
        private static void HttpDelete(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            UnityDebug.Log(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DebugMethod(alias: "http.patch")]
        private static void HttpPatch(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PATCH";

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            UnityDebug.Log(result);
        }
#endif

#if ENABLE_SYSTEM_COMMANDS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        [DebugMethod(alias: "system.openLink", altAlias: "system.startProcess")]
        private static void SystemProcessStart(string fileName) => Process.Start(fileName).Dispose();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        [DebugMethod(alias: "system.closeProcess")]
        private static void SystemProcessClose(string fileName)
        {
            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName));

            UnityDebug.Log(processes.Length);

            foreach (var process in processes)
            {
                UnityDebug.Log(process.ProcessName);

                process.CloseMainWindow();
                process.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        [DebugMethod(alias: "system.runCMD")]
        private static void SystemRunCMD(string arguments = "") => Process.Start("CMD.exe", arguments);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        [DebugMethod("system.openPath")]
        private static void SystemOpenPath(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            Process.Start("explorer.exe", "/select, " + filePath);
        }
#endif
    }
#endif
}
