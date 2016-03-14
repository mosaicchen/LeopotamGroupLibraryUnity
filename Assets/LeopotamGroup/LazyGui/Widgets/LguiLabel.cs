﻿//-------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2016 Leopotam <leopotam@gmail.com>
//-------------------------------------------------------

using LeopotamGroup.LazyGui.Core;
using UnityEngine;

namespace LeopotamGroup.LazyGui.Widgets {
    [ExecuteInEditMode]
    [RequireComponent (typeof (MeshFilter))]
    [RequireComponent (typeof (MeshRenderer))]
    public class LguiLabel : LguiVisualBase {
        public Font Font {
            get { return _font; }
            set {
                if (value != _font) {
                    _font = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public int FontSize {
            get { return _fontSize; }
            set {
                if (Mathf.Abs (value) != _fontSize) {
                    _fontSize = Mathf.Abs (value);
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public string Text {
            get { return _text; }
            set {
                if (value != _text) {
                    _text = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public TextAnchor Alignment {
            get { return _alignment; }
            set {
                if (value != _alignment) {
                    _alignment = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public float LineHeight {
            get { return _lineHeight; }
            set {
                if (LineHeight > 0 && System.Math.Abs (value - _lineHeight) > 0f) {
                    _lineHeight = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public SpriteEffect Effect {
            get { return _effect; }
            set {
                if (_effect != value) {
                    _effect = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public Vector2 EffectValue {
            get { return _effectValue; }
            set {
                if (_effectValue != value) {
                    _effectValue = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        public Color EffectColor {
            get { return _effectColor; }
            set {
                if (_effectColor != value) {
                    _effectColor = value;
                    AddVisualChanges (ChangeType.Geometry);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        TextAnchor _alignment = TextAnchor.MiddleCenter;

        [HideInInspector]
        [SerializeField]
        Font _font;

        [HideInInspector]
        [SerializeField]
        int _fontSize = 32;

        [Multiline (5)]
        [HideInInspector]
        [SerializeField]
        string _text;

        [HideInInspector]
        [SerializeField]
        float _lineHeight = 1f;

        [HideInInspector]
        [SerializeField]
        SpriteEffect _effect = SpriteEffect.None;

        [HideInInspector]
        [SerializeField]
        Vector2 _effectValue = Vector2.one;

        [HideInInspector]
        [SerializeField]
        Color _effectColor = Color.black;

        MeshFilter _meshFilter;

        protected override void Awake () {
            base.Awake ();
            _meshFilter = GetComponent<MeshFilter> ();
            _meshFilter.sharedMesh = null;
        }

        void OnEnable () {
            if (_meshFilter == null) {
                _meshFilter = GetComponent<MeshFilter> ();
            }
            _meshFilter.hideFlags = HideFlags.HideInInspector;

            if (_meshFilter.sharedMesh == null) {
                _meshFilter.sharedMesh = LguiMeshTools.GetNewMesh ();
            }

            _meshRenderer = GetComponent<MeshRenderer> ();
            _meshRenderer.hideFlags = HideFlags.HideInInspector;

            AddVisualChanges (ChangeType.All);

            Font.textureRebuilt += OnFontTextureRebuilt;
        }

        void OnDisable () {
            Font.textureRebuilt -= OnFontTextureRebuilt;
            _meshRenderer.enabled = false;
            _meshFilter = null;
            _meshRenderer = null;
            _visualPanel = null;
        }

        protected override bool NeedToUpdateVisuals (ChangeType changes) {
            if (!base.NeedToUpdateVisuals (changes)) {
                return false;
            }

            if ((changes & (ChangeType.Geometry | ChangeType.Panel | ChangeType.Color)) != ChangeType.None) {
                if (Font != null) {
                    _meshRenderer.sharedMaterial = _visualPanel.GetFontMaterial (Font);

                    if ((changes & (ChangeType.Geometry | ChangeType.Color)) != ChangeType.None) {
                        LguiTextTools.FillText (_meshFilter.sharedMesh, Width, Height, Text, Color, Alignment, Font, FontSize, LineHeight, _effect, _effectValue, _effectColor);
                    }
                }
            }
            return true;
        }

        void OnFontTextureRebuilt (Font changedFont) {
            if (changedFont == Font) {
                AddVisualChanges (ChangeType.All);
            }
        }
    }
}