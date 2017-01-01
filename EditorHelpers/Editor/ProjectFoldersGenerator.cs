﻿
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Project folders generator.
    /// </summary>
    sealed class ProjectFoldersGenerator : EditorWindow {
        [Flags]
        public enum Options {
            Animations = 1,
            Fonts = 2,
            Models = 4,
            Plugins = 8,
            Prefabs = 16,
            Resources = 32,
            Scenes = 64,
            Scripts = 128,
            Shaders = 256,
            Sounds = 512,
            StreamingAssets = 1024,
            Textures = 2048,
        }

        static readonly Dictionary<int, List<string>> _paths = new Dictionary<int, List<string>> {
            { (int) Options.Scripts, new List<string> { "Editor" } },
            { (int) Options.Textures, new List<string> { "AppIcon", "UI" } }
        };

        const string Title = "Project folders generator";

        const Options DefaultOptions = (Options) (-1);

        const Options RootOnlyOptions = Options.Plugins;

        const string DefaultRootProjectFolder = "Client";

        const string DefaultCvsFileName = ".keep";

        string _projectRootFolder;

        Options _options;

        bool _cvsSupport;

        string _cvsFileName;

        string[] _optionNames;

        [MenuItem ("Window/LeopotamGroupLibrary/Project folders generator...")]
        static void InitGeneration () {
            var win = GetWindow<ProjectFoldersGenerator> ();
            win.Reset ();
        }

        void Awake () {
            Reset ();
        }

        void OnEnable () {
            titleContent.text = Title;
        }

        void Reset () {
            _projectRootFolder = DefaultRootProjectFolder;
            _options = DefaultOptions;
            _cvsSupport = true;
            _cvsFileName = DefaultCvsFileName;
        }

        void OnGUI () {
            if (_optionNames == null) {
                _optionNames = Enum.GetNames (typeof (Options));
            }

            _projectRootFolder = EditorGUILayout.TextField ("Project root folder", _projectRootFolder).Trim ();

            _options = (Options) EditorGUILayout.MaskField ("Options", (int) _options, _optionNames);

            var cvsSupport = EditorGUILayout.Toggle ("Cvs support", _cvsSupport);
            if (cvsSupport != _cvsSupport) {
                _cvsSupport = cvsSupport;
                _cvsFileName = DefaultCvsFileName;
            }

            if (_cvsSupport) {
                _cvsFileName = EditorGUILayout.TextField ("Cvs filename", _cvsFileName).Trim ();
            }

            if (GUILayout.Button ("Reset settings")) {
                Reset ();
                Repaint ();
            }
            if (GUILayout.Button ("Generate")) {
                if (string.IsNullOrEmpty (_cvsFileName)) {
                    _cvsSupport = false;
                }
                var res = Generate (_projectRootFolder, _options, _cvsSupport ? _cvsFileName : null);
                EditorUtility.DisplayDialog (titleContent.text, res ?? "Success", "Close");
            }
        }

        static void GenerateCvsSupport (string path, string cvsFileName) {
            if (!string.IsNullOrEmpty (cvsFileName)) {
                path = Path.Combine (path, cvsFileName);
                if (!File.Exists (path)) {
                    File.WriteAllText (path, string.Empty);
                }
            }
        }

        static void GenerateItem (string rootFolder, int item, string cvsFileName) {
            var fullPath = (((int) RootOnlyOptions) & item) != 0 ?
                           Application.dataPath : Path.Combine (Application.dataPath, rootFolder);

            fullPath = Path.Combine (fullPath, ((Options) item).ToString ());
            if (!Directory.Exists (fullPath)) {
                Directory.CreateDirectory (fullPath);
            }

            if (_paths.ContainsKey (item)) {
                string path;
                foreach (var subFolder in _paths[item]) {
                    path = Path.Combine (fullPath, subFolder);
                    if (!Directory.Exists (path)) {
                        Directory.CreateDirectory (path);
                    }
                    GenerateCvsSupport (path, cvsFileName);
                }
            } else {
                GenerateCvsSupport (fullPath, cvsFileName);
            }
        }

        /// <summary>
        /// Generate class with idents at specified filename and with specified namespace.
        /// </summary>
        /// <returns>Error message or null on success.</returns>
        /// <param name="rootFolder">Root folder path or empty/null for disable.</param>
        /// <param name="options">Options</param>
        /// <param name="cvsFileName">Cvs filename for keep empty folders or null for disable.</param>
        public static string Generate (string rootFolder, Options options, string cvsFileName) {
            if ((int) options == 0) {
                return string.Empty;
            }
            if (rootFolder == null) {
                rootFolder = string.Empty;
            }
            try {
                foreach (Options item in Enum.GetValues (typeof (Options))) {
                    if ((int) (options & item) != 0) {
                        GenerateItem (rootFolder, (int) item, cvsFileName);
                    }
                }
                AssetDatabase.Refresh ();
                return null;
            } catch (Exception ex) {
                AssetDatabase.Refresh ();
                return ex.Message;
            }
        }
    }
}