using UnityEngine;
using UnityEngine.UIElements;

namespace RoyalTableSoccer.UI
{
    /// <summary>
    /// Drop-in spinner: rotates every UI Toolkit VisualElement that has the
    /// configured class name. Add this script next to a UIDocument and any
    /// element marked with the class will rotate automatically.
    ///
    /// - Class "rts-spinner" rotates clockwise.
    /// - Class "rts-spinner-reverse" rotates counter-clockwise.
    /// - You can add as many spinners as you want — they all share the same
    ///   speed defined here.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UISpinner : MonoBehaviour
    {
        [Tooltip("Class name for elements that rotate clockwise.")]
        [SerializeField] private string spinClass = "rts-spinner";

        [Tooltip("Class name for elements that rotate counter-clockwise. Leave empty to disable.")]
        [SerializeField] private string reverseClass = "rts-spinner-reverse";

        [Tooltip("Rotation speed in degrees per second.")]
        [SerializeField, Min(0f)] private float speedDegPerSec = 220f;

        [Tooltip("Use unscaled time so the spinner keeps moving even when the game is paused.")]
        [SerializeField] private bool unscaledTime = true;

        private VisualElement _root;
        private float _angle;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void Update()
        {
            if (_root == null) return;

            float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _angle = (_angle + speedDegPerSec * dt) % 360f;

            var cw  = new StyleRotate(new Rotate(_angle));
            var ccw = new StyleRotate(new Rotate(-_angle));

            if (!string.IsNullOrEmpty(spinClass))
                _root.Query<VisualElement>(className: spinClass).ForEach(e => e.style.rotate = cw);

            if (!string.IsNullOrEmpty(reverseClass))
                _root.Query<VisualElement>(className: reverseClass).ForEach(e => e.style.rotate = ccw);
        }
    }
}
