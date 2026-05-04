using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao3
{
    [RequireComponent(typeof(UIDocument))]
    public class ProfileController : MonoBehaviour
    {
        [Serializable]
        public class ProfileData
        {
            public string playerName = "MAFIAZOUK";
            public int level = 1;
            public int xpCurrent = 180;
            public int xpForNext = 1000;
            public int onlineWins;
            public int starLeagueWins;
            public int trophies;
            public int goals;
            public int rank;
            public string divisionName = "BRONZE I";
            public string divisionHint = "Vença 3 partidas para promover";
            public int promoCurrent;
            public int promoTarget = 3;
            public Sprite avatar;
            public Sprite badge;
        }

        [SerializeField] private ProfileData data = new();

        public event Action OnPickPhoto;
        public event Action OnTakePhoto;
        public event Action OnEditName;
        public event Action OnShare;
        public event Action OnBack;

        private VisualElement _root;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _root.Q<Button>("btn-back").clicked       += () => OnBack?.Invoke();
            _root.Q<Button>("btn-pick-photo").clicked += () => OnPickPhoto?.Invoke();
            _root.Q<Button>("btn-take-photo").clicked += () => OnTakePhoto?.Invoke();
            _root.Q<Button>("btn-edit-name").clicked  += () => OnEditName?.Invoke();
            _root.Q<Button>("btn-share").clicked      += () => OnShare?.Invoke();

            Refresh();
        }

        public void SetData(ProfileData newData)
        {
            data = newData;
            if (_root != null) Refresh();
        }

        public void SetAvatar(Sprite sprite)
        {
            data.avatar = sprite;
            if (_root != null)
                _root.Q<VisualElement>("avatar").style.backgroundImage = new StyleBackground(sprite);
        }

        private void Refresh()
        {
            _root.Q<Label>("lbl-name").text         = (data.playerName ?? "").ToUpperInvariant();
            _root.Q<Label>("lbl-level").text        = $"LEVEL {data.level}";
            _root.Q<Label>("lbl-xp").text           = $"{data.xpCurrent} / {data.xpForNext} XP";
            _root.Q<Label>("lbl-online-wins").text  = data.onlineWins.ToString();
            _root.Q<Label>("lbl-star-wins").text    = data.starLeagueWins.ToString();
            _root.Q<Label>("lbl-trophies").text     = data.trophies.ToString();
            _root.Q<Label>("lbl-goals").text        = data.goals.ToString();
            _root.Q<Label>("lbl-rank-chip").text    = $"RANK {data.rank}";
            _root.Q<Label>("lbl-division").text     = (data.divisionName ?? "").ToUpperInvariant();
            _root.Q<Label>("lbl-division-sub").text = data.divisionHint ?? "";

            float xpPct = data.xpForNext > 0
                ? Mathf.Clamp01((float)data.xpCurrent / data.xpForNext) * 100f : 0f;
            _root.Q<VisualElement>("xp-fill").style.width = new Length(xpPct, LengthUnit.Percent);

            float promoPct = data.promoTarget > 0
                ? Mathf.Clamp01((float)data.promoCurrent / data.promoTarget) * 100f : 0f;
            _root.Q<VisualElement>("promo-fill").style.width = new Length(promoPct, LengthUnit.Percent);

            if (data.avatar != null)
                _root.Q<VisualElement>("avatar").style.backgroundImage = new StyleBackground(data.avatar);
            if (data.badge != null)
                _root.Q<VisualElement>("badge").style.backgroundImage = new StyleBackground(data.badge);
        }
    }
}
