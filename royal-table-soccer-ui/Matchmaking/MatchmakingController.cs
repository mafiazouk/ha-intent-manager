using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MatchmakingController : MonoBehaviour
    {
        [Serializable]
        public class PlayerInfo
        {
            public string playerName;
            public int level;
            public int rank;
            public Sprite avatar;
        }

        [Header("Local player")]
        [SerializeField] private PlayerInfo localPlayer = new();

        [Header("Spinner")]
        [SerializeField, Min(0f)] private float spinnerSpeedDegPerSec = 220f;

        [Header("Tips (rotated every few seconds)")]
        [SerializeField] private List<string> tips = new()
        {
            "DICA: ABASTECA SEU TIME ANTES DAS PARTIDAS RANKEADAS",
            "DICA: FORMACAO 1-1-2-1 AUMENTA SEU PODER OFENSIVO",
            "DICA: VITORIAS CONSECUTIVAS DOBRAM SEUS PONTOS",
            "DICA: COMPLETE OBJETIVOS DIARIOS PARA GANHAR DIAMANTES"
        };
        [SerializeField, Min(1f)] private float tipRotateSeconds = 4f;

        public event Action OnCancel;
        public event Action<PlayerInfo> OnOpponentFound;

        private VisualElement _root;
        private VisualElement _spinnerL, _spinnerR;
        private Label _timerLabel, _tipLabel;
        private Label _rightName, _rightMeta, _question;
        private VisualElement _rightAvatar;

        private float _spinAngle;
        private float _elapsed;
        private float _tipTimer;
        private int _tipIndex;
        private bool _searching = true;
        private PlayerInfo _opponent;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _spinnerL = _root.Q<VisualElement>(className: "mm-spinner-left");
            _spinnerR = _root.Q<VisualElement>(className: "mm-spinner-right");
            _timerLabel = _root.Q<Label>("lbl-timer");
            _tipLabel   = _root.Q<Label>("lbl-tip");
            _rightName  = _root.Q<Label>("lbl-right-name");
            _rightMeta  = _root.Q<Label>("lbl-right-meta");
            _question   = _root.Q<Label>("lbl-question");
            _rightAvatar = _root.Q<VisualElement>("right-avatar");

            // Local player info
            _root.Q<Label>("lbl-left-name").text = (localPlayer.playerName ?? "JOGADOR").ToUpperInvariant();
            _root.Q<Label>("lbl-left-meta").text = $"LV {localPlayer.level}  ·  RANK {localPlayer.rank}";
            if (localPlayer.avatar != null)
                _root.Q<VisualElement>("left-avatar").style.backgroundImage = new StyleBackground(localPlayer.avatar);

            _root.Q<Button>("btn-cancel").clicked += () => OnCancel?.Invoke();

            _searching = true;
            _elapsed = 0f;
            _tipTimer = 0f;
            _tipIndex = 0;
            UpdateTip();
            UpdateTimer();
        }

        private void Update()
        {
            if (!_searching) return;

            float dt = Time.unscaledDeltaTime;

            // Spin both rings (opposite directions for visual interest)
            _spinAngle = (_spinAngle + spinnerSpeedDegPerSec * dt) % 360f;
            if (_spinnerL != null) _spinnerL.style.rotate = new StyleRotate(new Rotate(_spinAngle));
            if (_spinnerR != null) _spinnerR.style.rotate = new StyleRotate(new Rotate(-_spinAngle));

            // Timer
            _elapsed += dt;
            UpdateTimer();

            // Rotate tips
            _tipTimer += dt;
            if (_tipTimer >= tipRotateSeconds && tips.Count > 0)
            {
                _tipTimer = 0f;
                _tipIndex = (_tipIndex + 1) % tips.Count;
                UpdateTip();
            }
        }

        private void UpdateTimer()
        {
            int total = Mathf.FloorToInt(_elapsed);
            int mm = total / 60;
            int ss = total % 60;
            if (_timerLabel != null) _timerLabel.text = $"{mm:00} : {ss:00}";
        }

        private void UpdateTip()
        {
            if (tips.Count == 0 || _tipLabel == null) return;
            _tipLabel.text = tips[_tipIndex];
        }

        // ---- Public API ----
        public void SetLocalPlayer(PlayerInfo info)
        {
            localPlayer = info;
            if (_root != null) OnEnable();
        }

        /// <summary>Call when an opponent is found — fills the right side and stops the spinner.</summary>
        public void ShowOpponent(PlayerInfo opponent)
        {
            _opponent = opponent;
            _searching = false;

            if (_question != null) _question.style.display = DisplayStyle.None;
            if (_rightName != null) _rightName.text = (opponent.playerName ?? "OPONENTE").ToUpperInvariant();
            if (_rightMeta != null) _rightMeta.text = $"LV {opponent.level}  ·  RANK {opponent.rank}";

            if (_rightAvatar != null && opponent.avatar != null)
            {
                _rightAvatar.style.backgroundImage = new StyleBackground(opponent.avatar);
                _rightAvatar.RemoveFromClassList("mm-portrait-unknown");
            }

            OnOpponentFound?.Invoke(opponent);
        }

        public void ResetSearch()
        {
            _searching = true;
            _elapsed = 0f;
            _opponent = null;
            if (_question != null) _question.style.display = DisplayStyle.Flex;
            if (_rightAvatar != null) _rightAvatar.AddToClassList("mm-portrait-unknown");
            if (_rightName != null) _rightName.text = "PROCURANDO...";
            if (_rightMeta != null) _rightMeta.text = "—";
        }
    }
}
