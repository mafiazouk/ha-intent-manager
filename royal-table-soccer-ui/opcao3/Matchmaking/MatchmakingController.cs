using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao3
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

        [Header("Animations")]
        [SerializeField, Min(0f)] private float ringSpeedDegPerSec = 90f;
        [SerializeField, Min(0.1f)] private float pulseSeconds = 1.4f;
        [SerializeField, Min(0.1f)] private float dotPulseSeconds = 1.0f;

        [Header("Tips (rotated)")]
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
        private VisualElement _ringL, _ringR, _vsPulse, _tipDot, _rightPhoto;
        private Label _timerLabel, _tipLabel, _question, _rightName, _rightMeta;

        private float _ringAngle;
        private float _pulseT;
        private float _dotT;
        private float _elapsed;
        private float _tipTimer;
        private int _tipIndex;
        private bool _searching = true;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _ringL = _root.Q<VisualElement>("ring-left");
            _ringR = _root.Q<VisualElement>("ring-right");
            _vsPulse = _root.Q<VisualElement>("vs-pulse");
            _tipDot = _root.Q<VisualElement>("tip-dot");
            _rightPhoto = _root.Q<VisualElement>("right-photo");

            _timerLabel = _root.Q<Label>("lbl-timer");
            _tipLabel   = _root.Q<Label>("lbl-tip");
            _question   = _root.Q<Label>("lbl-question");
            _rightName  = _root.Q<Label>("lbl-right-name");
            _rightMeta  = _root.Q<Label>("lbl-right-meta");

            _root.Q<Label>("lbl-left-name").text = (localPlayer.playerName ?? "JOGADOR").ToUpperInvariant();
            _root.Q<Label>("lbl-left-meta").text = $"LV {localPlayer.level} - RANK {localPlayer.rank}";
            if (localPlayer.avatar != null)
                _root.Q<VisualElement>("left-photo").style.backgroundImage = new StyleBackground(localPlayer.avatar);

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
            float dt = Time.unscaledDeltaTime;

            // Hex rings spin in opposite directions
            _ringAngle = (_ringAngle + ringSpeedDegPerSec * dt) % 360f;
            if (_ringL != null) _ringL.style.rotate = new StyleRotate(new Rotate(_ringAngle));
            if (_ringR != null) _ringR.style.rotate = new StyleRotate(new Rotate(-_ringAngle));

            // VS pulse bar (scaleX 0.4 -> 1.0 -> 0.4)
            _pulseT = (_pulseT + dt) % pulseSeconds;
            float ps = Mathf.Lerp(0.4f, 1f, Mathf.Sin(_pulseT / pulseSeconds * Mathf.PI));
            if (_vsPulse != null)
                _vsPulse.style.scale = new StyleScale(new Scale(new Vector3(ps, 1f, 1f)));

            // Tip dot pulse
            _dotT = (_dotT + dt) % dotPulseSeconds;
            float ds = Mathf.Lerp(0.7f, 1.2f, 0.5f * (1f - Mathf.Cos(_dotT / dotPulseSeconds * Mathf.PI * 2f)));
            if (_tipDot != null)
                _tipDot.style.scale = new StyleScale(new Scale(new Vector3(ds, ds, 1f)));

            if (!_searching) return;

            _elapsed += dt;
            UpdateTimer();

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
            if (_timerLabel != null) _timerLabel.text = $"{total / 60:00} : {total % 60:00}";
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

        public void ShowOpponent(PlayerInfo opponent)
        {
            _searching = false;

            if (_question != null) _question.style.display = DisplayStyle.None;
            if (_rightName != null) _rightName.text = (opponent.playerName ?? "OPONENTE").ToUpperInvariant();
            if (_rightMeta != null) _rightMeta.text = $"LV {opponent.level} - RANK {opponent.rank}";

            if (_rightPhoto != null && opponent.avatar != null)
            {
                _rightPhoto.style.backgroundImage = new StyleBackground(opponent.avatar);
                _rightPhoto.RemoveFromClassList("mm3-photo-unknown");
            }

            OnOpponentFound?.Invoke(opponent);
        }

        public void ResetSearch()
        {
            _searching = true;
            _elapsed = 0f;
            if (_question != null) _question.style.display = DisplayStyle.Flex;
            if (_rightPhoto != null) _rightPhoto.AddToClassList("mm3-photo-unknown");
            if (_rightName != null) _rightName.text = "PROCURANDO";
            if (_rightMeta != null) _rightMeta.text = "—";
        }
    }
}
