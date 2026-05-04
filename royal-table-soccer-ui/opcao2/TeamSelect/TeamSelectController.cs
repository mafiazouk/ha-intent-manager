using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao2
{
    [RequireComponent(typeof(UIDocument))]
    public class TeamSelectController : MonoBehaviour
    {
        [Serializable]
        public class TeamData
        {
            public string name;
            public Sprite crest;
            public Sprite countryFlag;
            public Sprite leagueBadge;
            [Range(0, 99)] public int attack = 75;
            [Range(0, 99)] public int midfield = 75;
            [Range(0, 99)] public int defense = 75;
            [Range(0, 99)] public int pace = 75;
            [Range(0, 99)] public int stamina = 75;
            [Range(0, 99)] public int morale = 75;

            public int Overall => Mathf.RoundToInt((attack + midfield + defense + pace + stamina + morale) / 6f);
        }

        [Serializable]
        public class FormationData
        {
            public string label = "1-2-1-1";
            public int[] linePlayers = new[] { 1, 2, 1, 1 };
        }

        [Header("Data")]
        [SerializeField] private List<TeamData> teams = new();
        [SerializeField] private List<FormationData> formations = new();
        [SerializeField] private string leagueName = "PRIMERA DIVISION";

        public event Action<TeamData, FormationData> Confirmed;
        public event Action Cancelled;

        private VisualElement _root;
        private int _teamIndex;
        private int _formationIndex;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _root.Q<Button>("btn-back").clicked      += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-cancel").clicked    += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-confirm").clicked   += OnConfirm;
            _root.Q<Button>("btn-prev-team").clicked += () => Cycle(ref _teamIndex, teams.Count, -1, RefreshTeam);
            _root.Q<Button>("btn-next-team").clicked += () => Cycle(ref _teamIndex, teams.Count, +1, RefreshTeam);
            _root.Q<Button>("btn-prev-form").clicked += () => Cycle(ref _formationIndex, formations.Count, -1, RefreshFormation);
            _root.Q<Button>("btn-next-form").clicked += () => Cycle(ref _formationIndex, formations.Count, +1, RefreshFormation);

            _root.Q<Label>("lbl-league").text = leagueName;

            RefreshTeam();
            RefreshFormation();
        }

        private static void Cycle(ref int index, int count, int delta, Action onChange)
        {
            if (count == 0) return;
            index = (index + delta + count) % count;
            onChange?.Invoke();
        }

        private void RefreshTeam()
        {
            if (teams.Count == 0) return;
            var t = teams[_teamIndex];

            _root.Q<Label>("lbl-team-name").text = t.name?.ToUpperInvariant() ?? "—";
            _root.Q<Label>("lbl-team-ovr").text  = t.Overall.ToString();

            SetSprite("team-crest", t.crest);
            SetSprite("team-country", t.countryFlag);
            SetSprite("team-league", t.leagueBadge);

            _root.Q<Label>("stat-att").text     = t.attack.ToString();
            _root.Q<Label>("stat-mid").text     = t.midfield.ToString();
            _root.Q<Label>("stat-def").text     = t.defense.ToString();
            _root.Q<Label>("stat-pace").text    = t.pace.ToString();
            _root.Q<Label>("stat-stamina").text = t.stamina.ToString();
            _root.Q<Label>("stat-morale").text  = t.morale.ToString();

            // Side previews: prev2/prev3 = neighbours, prev1/prev4 = +/-2
            int n = teams.Count;
            FillPreview("prev-1", teams[(_teamIndex - 2 + n) % n]);
            FillPreview("prev-2", teams[(_teamIndex - 1 + n) % n]);
            FillPreview("prev-3", teams[(_teamIndex + 1) % n]);
            FillPreview("prev-4", teams[(_teamIndex + 2) % n]);
        }

        private void FillPreview(string baseName, TeamData t)
        {
            SetSprite(baseName + "-crest", t.crest);
            _root.Q<Label>(baseName + "-ovr").text  = t.Overall.ToString();
            _root.Q<Label>(baseName + "-name").text = t.name?.ToUpperInvariant() ?? "—";
        }

        private void SetSprite(string elementName, Sprite sprite)
        {
            if (sprite == null) return;
            var ve = _root.Q<VisualElement>(elementName);
            if (ve != null) ve.style.backgroundImage = new StyleBackground(sprite);
        }

        private void RefreshFormation()
        {
            if (formations.Count == 0) return;
            var f = formations[_formationIndex];
            _root.Q<Label>("lbl-formation").text = string.Join(" - ", f.label.Split('-'));
        }

        private void OnConfirm()
        {
            if (teams.Count == 0 || formations.Count == 0) return;
            Confirmed?.Invoke(teams[_teamIndex], formations[_formationIndex]);
        }

        public void SetTeams(IEnumerable<TeamData> data)
        {
            teams = new List<TeamData>(data);
            _teamIndex = 0;
            if (_root != null) RefreshTeam();
        }

        public void SetFormations(IEnumerable<FormationData> data)
        {
            formations = new List<FormationData>(data);
            _formationIndex = 0;
            if (_root != null) RefreshFormation();
        }
    }
}
