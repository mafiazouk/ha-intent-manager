using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI
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

        public event Action<TeamData, FormationData> Confirmed;
        public event Action Cancelled;

        private VisualElement _root;
        private Label _teamName;
        private VisualElement _crest;
        private Label _formationLabel;
        private VisualElement _pitch;

        private VisualElement _statAtk, _statMid, _statDef;
        private Label _statAtkVal, _statMidVal, _statDefVal;

        private int _teamIndex;
        private int _formationIndex;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _teamName       = _root.Q<Label>("lbl-team-name");
            _crest          = _root.Q<VisualElement>("team-crest");
            _formationLabel = _root.Q<Label>("lbl-formation");
            _pitch          = _root.Q<VisualElement>("pitch");

            _statAtk = _root.Q<VisualElement>("stat-atk");
            _statMid = _root.Q<VisualElement>("stat-mid");
            _statDef = _root.Q<VisualElement>("stat-def");
            _statAtkVal = _root.Q<Label>("stat-atk-val");
            _statMidVal = _root.Q<Label>("stat-mid-val");
            _statDefVal = _root.Q<Label>("stat-def-val");

            _root.Q<Button>("btn-prev-team").clicked  += () => Cycle(ref _teamIndex, teams.Count, -1, RefreshTeam);
            _root.Q<Button>("btn-next-team").clicked  += () => Cycle(ref _teamIndex, teams.Count, +1, RefreshTeam);
            _root.Q<Button>("btn-prev-form").clicked  += () => Cycle(ref _formationIndex, formations.Count, -1, RefreshFormation);
            _root.Q<Button>("btn-next-form").clicked  += () => Cycle(ref _formationIndex, formations.Count, +1, RefreshFormation);
            _root.Q<Button>("btn-back").clicked       += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-cancel").clicked     += () => Cancelled?.Invoke();
            _root.Q<Button>("btn-confirm").clicked    += OnConfirm;

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
            _teamName.text = t.name?.ToUpperInvariant() ?? "—";

            if (t.crest != null)
                _crest.style.backgroundImage = new StyleBackground(t.crest);

            SetStat(_statAtk, _statAtkVal, t.attack);
            SetStat(_statMid, _statMidVal, t.midfield);
            SetStat(_statDef, _statDefVal, t.defense);
        }

        private static void SetStat(VisualElement bar, Label valueLabel, int value)
        {
            value = Mathf.Clamp(value, 0, 99);
            bar.style.width = new Length(value, LengthUnit.Percent);
            valueLabel.text = value.ToString();
        }

        private void RefreshFormation()
        {
            if (formations.Count == 0) return;
            var f = formations[_formationIndex];
            _formationLabel.text = string.Join(" - ", f.label.Split('-'));
            BuildPitchMarkers(f);
        }

        private void BuildPitchMarkers(FormationData f)
        {
            _pitch.Clear();
            foreach (var n in f.linePlayers)
            {
                var line = new VisualElement();
                line.AddToClassList("ts-pitch-line");
                for (int i = 0; i < n; i++)
                {
                    var marker = new VisualElement();
                    marker.AddToClassList("ts-pitch-marker");
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

        // ---- Public API for runtime data injection ----
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
