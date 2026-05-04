using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI.Opcao3
{
    [RequireComponent(typeof(UIDocument))]
    public class TeamSelectController : MonoBehaviour
    {
        [Serializable]
        public class TeamData
        {
            public string name;
            public Sprite crest;
            [Range(0, 99)] public int attack = 75;
            [Range(0, 99)] public int midfield = 75;
            [Range(0, 99)] public int defense = 75;
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
        [SerializeField] private string leagueName = "LIGA: PRIMERA";

        public event Action<TeamData, FormationData> Confirmed;
        public event Action Cancelled;

        private VisualElement _root;
        private VisualElement _pitch;
        private int _teamIndex;
        private int _formationIndex;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _pitch = _root.Q<VisualElement>("pitch");

            _root.Q<Label>("lbl-league").text = leagueName;

            _root.Q<Button>("btn-back").clicked      += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-cancel").clicked    += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-confirm").clicked   += OnConfirm;
            _root.Q<Button>("btn-prev-team").clicked += () => Cycle(ref _teamIndex, teams.Count, -1, RefreshTeam);
            _root.Q<Button>("btn-next-team").clicked += () => Cycle(ref _teamIndex, teams.Count, +1, RefreshTeam);
            _root.Q<Button>("btn-prev-form").clicked += () => Cycle(ref _formationIndex, formations.Count, -1, RefreshFormation);
            _root.Q<Button>("btn-next-form").clicked += () => Cycle(ref _formationIndex, formations.Count, +1, RefreshFormation);

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

            if (t.crest != null)
                _root.Q<VisualElement>("team-crest").style.backgroundImage = new StyleBackground(t.crest);

            SetStat("stat-atk", "stat-atk-val", t.attack);
            SetStat("stat-mid", "stat-mid-val", t.midfield);
            SetStat("stat-def", "stat-def-val", t.defense);
        }

        private void SetStat(string fillName, string valueName, int value)
        {
            value = Mathf.Clamp(value, 0, 99);
            var fill = _root.Q<VisualElement>(fillName);
            if (fill != null) fill.style.width = new Length(value, LengthUnit.Percent);
            var lbl = _root.Q<Label>(valueName);
            if (lbl != null) lbl.text = value.ToString();
        }

        private void RefreshFormation()
        {
            if (formations.Count == 0) return;
            var f = formations[_formationIndex];
            _root.Q<Label>("lbl-formation").text = string.Join(" - ", f.label.Split('-'));
            BuildPitchMarkers(f);
        }

        private void BuildPitchMarkers(FormationData f)
        {
            _pitch.Clear();
            foreach (var n in f.linePlayers)
            {
                var line = new VisualElement();
                line.AddToClassList("ts3-pitch-line");
                for (int i = 0; i < n; i++)
                {
                    var marker = new VisualElement();
                    marker.AddToClassList("ts3-pitch-marker");
                    line.Add(marker);
                }
                _pitch.Add(line);
            }
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
