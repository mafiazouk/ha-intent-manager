using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao2
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
            public int overall;
            public int wins;
            public string formStreak = "—";
            public Sprite photo;
        }

        [Header("Local player")]
        [SerializeField] private PlayerInfo localPlayer = new();

        [Header("Pulse animation")]
        [SerializeField, Min(0.1f)] private float pulseSeconds = 1.4f;

        [Header("Status messages (rotated)")]
        [SerializeField] private List<string> statuses = new()
        {
            "CALIBRANDO HABILIDADE...",
            "ANALISANDO RANK...",
            "CONECTANDO REGIAO...",
            "FINALIZANDO PAREAMENTO..."
        };
        [SerializeField, Min(1f)] private float statusRotateSeconds = 4f;

        public event Action OnCancel;
        public event Action<PlayerInfo> OnOpponentFound;

        private VisualElement _root;
        private VisualElement _pulse, _rightPhoto;
        private Label _timerLabel, _statusLabel, _question;
        private Label _rightName, _rightMeta;
        private Label _cmpR1, _cmpR2, _cmpR3;

        private float _elapsed;
        private float _statusTimer;
        private int _statusIndex;
        private float _pulseTimer;
        private bool _searching = true;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _pulse        = _root.Q<VisualElement>("mm2-pulse");
            _rightPhoto   = _root.Q<VisualElement>("right-photo");
            _timerLabel   = _root.Q<Label>("lbl-timer");
            _statusLabel  = _root.Q<Label>("lbl-status");
            _question     = _root.Q<Label>("lbl-question");
            _rightName    = _root.Q<Label>("lbl-right-name");
            _rightMeta    = _root.Q<Label>("lbl-right-meta");
            _cmpR1        = _root.Q<Label>("lbl-cmp-right-1");
            _cmpR2        = _root.Q<Label>("lbl-cmp-right-2");
            _cmpR3        = _root.Q<Label>("lbl-cmp-right-3");

            _root.Q<Label>("lbl-left-name").text = (localPlayer.playerName ?? "JOGADOR").ToUpperInvariant();
            _root.Q<Label>("lbl-left-meta").text = $"LV {localPlayer.level} - RANK {localPlayer.rank}";
            _root.Q<Label>("lbl-cmp-left-1").text = localPlayer.overall.ToString();
            _root.Q<Label>("lbl-cmp-left-2").text = localPlayer.wins.ToString();
            _root.Q<Label>("lbl-cmp-left-3").text = localPlayer.formStreak ?? "—";

            if (localPlayer.photo != null)
                _root.Q<VisualElement>("left-photo").style.backgroundImage = new StyleBackground(localPlayer.photo);

            _root.Q<Button>("btn-cancel").clicked += () => OnCancel?.Invoke();

            _searching = true;
            _elapsed = 0f;
            _statusTimer = 0f;
            _statusIndex = 0;
            _pulseTimer = 0f;
            UpdateStatus();
            UpdateTimer();
        }

        private void Update()
        {
            if (!_searching) return;

            float dt = Time.unscaledDeltaTime;

            // pulse bar (scaleX 0.4 -> 1.0 -> 0.4)
            _pulseTimer = (_pulseTimer + dt) % pulseSeconds;
            float t = _pulseTimer / pulseSeconds;
            float s = Mathf.Lerp(0.4f, 1f, Mathf.Sin(t * Mathf.PI));
            if (_pulse != null)
                _pulse.style.scale = new StyleScale(new Scale(new Vector3(s, 1f, 1f)));

            _elapsed += dt;
            UpdateTimer();

            _statusTimer += dt;
            if (_statusTimer >= statusRotateSeconds && statuses.Count > 0)
            {
                _statusTimer = 0f;
                _statusIndex = (_statusIndex + 1) % statuses.Count;
                UpdateStatus();
            }
        }

        private void UpdateTimer()
        {
            int total = Mathf.FloorToInt(_elapsed);
            int mm = total / 60;
            int ss = total % 60;
            if (_timerLabel != null) _timerLabel.text = $"{mm:00} : {ss:00}";
        }

        private void UpdateStatus()
        {
            if (statuses.Count == 0 || _statusLabel == null) return;
            _statusLabel.text = statuses[_statusIndex];
        }

        // ---- Public API ----
        public void SetLocalPlayer(PlayerInfo info)
        {
            localPlayer = info;
            if (_root != null) OnEnable();
        }

        public void ShowOpponent(PlayerInfo opponent)
        {
            _searching = false;

            if (_question != null) _question.style.display = DisplayStyle.None;
            if (_rightName != null) _rightName.text = (opponent.playerName ?? "OPONENTE").ToUpperInvariant();
            if (_rightMeta != null) _rightMeta.text = $"LV {opponent.level} - RANK {opponent.rank}";
            if (_cmpR1 != null) _cmpR1.text = opponent.overall.ToString();
            if (_cmpR2 != null) _cmpR2.text = opponent.wins.ToString();
            if (_cmpR3 != null) _cmpR3.text = opponent.formStreak ?? "—";

            if (_rightPhoto != null && opponent.photo != null)
            {
                _rightPhoto.style.backgroundImage = new StyleBackground(opponent.photo);
                _rightPhoto.RemoveFromClassList("mm2-fighter-photo-unknown");
            }

            OnOpponentFound?.Invoke(opponent);
        }

        public void ResetSearch()
        {
            _searching = true;
            _elapsed = 0f;
            if (_question != null) _question.style.display = DisplayStyle.Flex;
            if (_rightPhoto != null) _rightPhoto.AddToClassList("mm2-fighter-photo-unknown");
            if (_rightName != null) _rightName.text = "PROCURANDO";
            if (_rightMeta != null) _rightMeta.text = "—";
            if (_cmpR1 != null) _cmpR1.text = "??";
            if (_cmpR2 != null) _cmpR2.text = "??";
            if (_cmpR3 != null) _cmpR3.text = "??";
        }
    }
}
