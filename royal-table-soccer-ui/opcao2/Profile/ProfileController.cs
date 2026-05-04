using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao2
{
    [RequireComponent(typeof(UIDocument))]
    public class ProfileController : MonoBehaviour
    {
        public enum Result { Win, Draw, Loss }

        [Serializable]
        public class Achievement
        {
            public string label;
            public string icon = "*";
            public Tier tier = Tier.Gold;
            public bool unlocked = true;

            public enum Tier { Gold, Silver, Bronze, Locked }
        }

        [Serializable]
        public class ProfileData
        {
            public string playerName = "MAFIAZOUK";
            public int level = 1;
            public int overall = 68;
            public int xp = 120;
            public int onlineWins;
            public int starLeagueWins;
            public int trophies;
            public int goals;
            public int streak;
            public int rank;
            public string divisionName = "BRONZE I";
            public Sprite photo;
            public Sprite countryFlag;
            public Sprite leagueBadge;
            public List<Achievement> achievements = new();
            public List<Result> lastMatches = new();
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

        public void SetPhoto(Sprite sprite)
        {
            data.photo = sprite;
            if (_root != null)
                _root.Q<VisualElement>("coach-photo").style.backgroundImage = new StyleBackground(sprite);
        }

        private void Refresh()
        {
            _root.Q<Label>("lbl-name").text         = (data.playerName ?? "").ToUpperInvariant();
            _root.Q<Label>("lbl-coach-ovr").text    = data.overall.ToString();
            _root.Q<Label>("lbl-rank-chip").text    = $"RANK {data.rank} - {data.divisionName.ToUpperInvariant()}";

            _root.Q<Label>("lbl-level").text          = data.level.ToString();
            _root.Q<Label>("lbl-goals-mini").text     = data.goals.ToString();
            _root.Q<Label>("lbl-online-mini").text    = data.onlineWins.ToString();
            _root.Q<Label>("lbl-trophies-mini").text  = data.trophies.ToString();
            _root.Q<Label>("lbl-streak-mini").text    = data.streak.ToString();
            _root.Q<Label>("lbl-xp-mini").text        = data.xp.ToString();

            _root.Q<Label>("lbl-online-wins").text = data.onlineWins.ToString();
            _root.Q<Label>("lbl-star-wins").text   = data.starLeagueWins.ToString();
            _root.Q<Label>("lbl-trophies").text    = data.trophies.ToString();
            _root.Q<Label>("lbl-goals").text       = data.goals.ToString();

            if (data.photo != null)
                _root.Q<VisualElement>("coach-photo").style.backgroundImage = new StyleBackground(data.photo);
            if (data.countryFlag != null)
                _root.Q<VisualElement>("coach-country").style.backgroundImage = new StyleBackground(data.countryFlag);
            if (data.leagueBadge != null)
                _root.Q<VisualElement>("coach-league").style.backgroundImage = new StyleBackground(data.leagueBadge);

            RefreshAchievements();
            RefreshFormStrip();
        }

        private void RefreshAchievements()
        {
            for (int i = 0; i < 6; i++)
            {
                var ve = _root.Q<VisualElement>($"ach-{i + 1}");
                if (ve == null) continue;

                ve.RemoveFromClassList("pf2-ach-silver");
                ve.RemoveFromClassList("pf2-ach-bronze");
                ve.RemoveFromClassList("pf2-ach-locked");

                if (i >= data.achievements.Count)
                {
                    ve.AddToClassList("pf2-ach-locked");
                    SetAchIcon(ve, "x");
                    continue;
                }

                var a = data.achievements[i];
                if (!a.unlocked) { ve.AddToClassList("pf2-ach-locked"); SetAchIcon(ve, "x"); continue; }

                switch (a.tier)
                {
                    case Achievement.Tier.Silver: ve.AddToClassList("pf2-ach-silver"); break;
                    case Achievement.Tier.Bronze: ve.AddToClassList("pf2-ach-bronze"); break;
                    case Achievement.Tier.Locked: ve.AddToClassList("pf2-ach-locked"); break;
                }
                SetAchIcon(ve, a.icon ?? "*");
            }
        }

        private static void SetAchIcon(VisualElement square, string icon)
        {
            var lbl = square.Q<Label>(className: "pf2-ach-icon");
            if (lbl != null) lbl.text = icon;
        }

        private void RefreshFormStrip()
        {
            int w = 0, d = 0, l = 0;
            for (int i = 0; i < 5; i++)
            {
                var sq = _root.Q<VisualElement>($"form-{i + 1}");
                if (sq == null) continue;

                sq.RemoveFromClassList("pf2-form-W");
                sq.RemoveFromClassList("pf2-form-D");
                sq.RemoveFromClassList("pf2-form-L");

                if (i >= data.lastMatches.Count)
                {
                    sq.AddToClassList("pf2-form-D");
                    var ll = sq.Q<Label>(className: "pf2-form-letter");
                    if (ll != null) ll.text = "-";
                    continue;
                }

                var r = data.lastMatches[i];
                string letter; string cls;
                switch (r)
                {
                    case Result.Win:  letter = "V"; cls = "pf2-form-W"; w++; break;
                    case Result.Draw: letter = "E"; cls = "pf2-form-D"; d++; break;
                    default:          letter = "D"; cls = "pf2-form-L"; l++; break;
                }
                sq.AddToClassList(cls);
                var lbl = sq.Q<Label>(className: "pf2-form-letter");
                if (lbl != null) lbl.text = letter;
            }

            var summary = _root.Q<Label>("lbl-form-summary");
            if (summary != null) summary.text = $"{w}V - {d}E - {l}D";
        }
    }
}
