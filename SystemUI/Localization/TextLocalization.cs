﻿
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using LeopotamGroup.Localization;
using UnityEngine.UI;
using UnityEngine;

namespace LeopotamGroup.SystemUI.Localization {
    /// <summary>
    /// Localization helper for System UI Text.
    /// </summary>
    [RequireComponent (typeof (Text))]
    public sealed class TextLocalization : MonoBehaviour {
        [SerializeField]
        string _token;

        Text _text;

        void OnEnable () {
            OnLocalize ();
        }

        void OnLocalize () {
            if (!string.IsNullOrEmpty (_token)) {
                if ((object) _text == null) {
                    _text = GetComponent<Text> ();
                }
                _text.text = Localizer.Get (_token);
            }
        }
    }
}