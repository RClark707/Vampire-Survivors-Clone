using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Terresquall {
    public class SavePoint : MonoBehaviour {

        [Tooltip("When checked, saving the game does not pause gameplay.")]
        public bool asynchronous = true;
        public enum DetectionMode { tagName, componentName }
        public DetectionMode detectionMode = DetectionMode.tagName;
        public string detectionTarget = "Player";

        protected readonly List<Component> objectsInRange = new List<Component>();

        [Header("Controls")]
#if ENABLE_INPUT_SYSTEM
        public InputAction interactKeys = new InputAction("Interact", binding: "<Keyboard>/e", type: InputActionType.Button);
#else
        public KeyCode[] interactKeys = { KeyCode.E };
#endif

        [Header("Feedback")]
        [Tooltip("Color of the object when someone is in range.")]
        public Color activeColor = new Color(.8f, .8f, .8f);
        [Tooltip("List of all Renderers this feedback should affect.")]
        public Renderer[] feedbackTargets;
        protected readonly Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

        // Delegates for other scripts to attach callbacks to.
        public event Action<Component> OnRangeEntry, OnRangeExit;

        // 1-frame override triggered by Interact().
        protected bool interactedInThisFrame = false;

        protected virtual void Start() {
            // Get all renderers attached to this save point.
            foreach (Renderer r in feedbackTargets) {
                if (r is SpriteRenderer sr) originalColors.Add(r, sr.color);
                else originalColors.Add(r, r.sharedMaterial.color);
            }

            // Check if there are any trigger colliders. If not, print a messsage.
            Collider2D[] col2D = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D c in col2D)
                if (c.isTrigger) return;
            Collider[] col = GetComponentsInChildren<Collider>();
            foreach (Collider c in col)
                if (c.isTrigger) return;

            Debug.LogWarning($"No collider found in Save Point <{name}>. It will not work.");
        }

        protected virtual void Reset() {
            feedbackTargets = GetComponentsInChildren<Renderer>();
        }

        public virtual void Interact() { interactedInThisFrame = true; }
        // Cancels an interaction triggered by OnInteract().
        public virtual void CancelInteract() { interactedInThisFrame= false; }

        public virtual bool CheckInteractKeyPress() {
#if ENABLE_INPUT_SYSTEM
            return interactKeys.WasPressedThisFrame();
#else
            foreach (KeyCode k in interactKeys) {
                if (Input.GetKeyDown(k)) 
                    return true;
            }
            return false;
#endif
        }

        protected virtual void Update() {
            if (objectsInRange.Count > 0 && (CheckInteractKeyPress() || interactedInThisFrame)) {
                if (asynchronous) Bench.SaveGameAsync();
                else Bench.SaveGame();
            }
            interactedInThisFrame = false;
        }

        // Checks if a component is a valid target, according to the settings in the component.
        public bool IsValidTarget(Component other) {
            switch (detectionMode) {
                case DetectionMode.tagName:
                    return other.CompareTag(detectionTarget);
                case DetectionMode.componentName:
                    Type type = Type.GetType(detectionTarget);
                    if (type != null) {
                        return other.GetComponent(type) != null;
                    } else {
                        Debug.LogWarning("Class entered for Save Point was not found. Please ensure you use the fully-qualified class name.");
                        return false;
                    }
            }
            return false;
        }

        protected virtual bool HandleRangeEntry(Component other) {
            if (IsValidTarget(other)) {
                if (!objectsInRange.Contains(other)) {
                    objectsInRange.Add(other);
                    foreach (Renderer r in feedbackTargets) {
                        if (r is SpriteRenderer sr) sr.color = activeColor;
                        else r.material.color = activeColor;
                    }

                    // Fire any attached callback events.
                    OnRangeEntry?.Invoke(other);
                    return true;
                }                
            }
            return false;
        }

        protected virtual bool HandleRangeExit(Component other) {
            if (IsValidTarget(other)) {
                if (objectsInRange.Contains(other)) {
                    objectsInRange.Remove(other);
                    foreach (Renderer r in feedbackTargets) {
                        if (r is SpriteRenderer sr) sr.color = originalColors[r];
                        else r.material.color = originalColors[r];
                    }

                    // Fire any attached callback events.
                    OnRangeExit?.Invoke(other);

                    return true;
                }
            }
            return false;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit2D(Collider2D other) { HandleRangeExit(other); }
        protected virtual void OnTriggerEnter(Collider other) { HandleRangeEntry(other); }
        protected virtual void OnTriggerExit(Collider other) { HandleRangeExit(other); }
    }
}