using UnityEngine;
using System.Linq;

[RequireComponent(typeof(EntityEventDispatcher))]
public class EntityDebugInspector : MonoBehaviour
{
    private EntityEventDispatcher dispatcher;
    private StatsBase stats;

    // You can tweak these to taste
    [Header("Debug Display Settings")]
    public int fontSize = 18;
    public Color textColor = Color.white;
    public Vector2 windowPos = new Vector2(10, 10);
    public Vector2 windowSize = new Vector2(400, 600);

    private GUIStyle labelStyle;
    private GUIStyle headerStyle;

    private void Awake()
    {
        dispatcher = GetComponent<EntityEventDispatcher>();
        stats = GetComponent<StatsBase>();
    }

    private void InitStyles()
    {
        if (labelStyle != null) return;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = fontSize,
            normal = { textColor = textColor },
            wordWrap = false
        };

        headerStyle = new GUIStyle(labelStyle)
        {
            fontStyle = FontStyle.Bold,
            fontSize = fontSize + 2,
            normal = { textColor = Color.yellow }
        };
    }

    private void OnGUI()
    {
        if (!DebugFlags.ShowEffectDebug) return;

        InitStyles();

        GUILayout.BeginArea(new Rect(windowPos.x, windowPos.y, windowSize.x, windowSize.y),
            "Entity Debug", GUI.skin.window);

        GUILayout.Label($"Entity: {name}", headerStyle);
        GUILayout.Space(10);

        // --- STATUS EFFECTS ---
        GUILayout.Label("Status Effects:", headerStyle);
        var effects = GetEffects().ToList();
        if (effects.Count == 0)
            GUILayout.Label(" (none)", labelStyle);
        else
        {
            foreach (var eff in effects)
            {
                GUILayout.Label($"- {eff.GetType().Name}  |  {eff.startTime:F1}/{eff.duration:F1}", labelStyle);
            }
        }

        GUILayout.Space(15);

        // --- STATS ---
        if (stats != null)
        {
            GUILayout.Label("Stats:", headerStyle);
            foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
            {
                GUILayout.Label($"{type,-15}: {stats.GetVal(type):F2}", labelStyle);
            }
        }

        GUILayout.EndArea();
    }

    private System.Collections.Generic.IEnumerable<StatusEffect> GetEffects()
    {
        // access private field through reflection
        var fi = typeof(EntityEventDispatcher).GetField("effects",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        return fi?.GetValue(dispatcher) as System.Collections.Generic.List<StatusEffect>
               ?? Enumerable.Empty<StatusEffect>();
    }
}
