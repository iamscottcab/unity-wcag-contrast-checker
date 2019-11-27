using UnityEditor;
using UnityEngine;

public class ColorAccessibilityChecker : EditorWindow
{
    // Constants
    private const string WINDOW_NAME = "WCAG Contrast Checker";
    private const string MENU_ITEM_NAME = "Accessibility/WCAG Contrast Checker";
    private const float WCAG_AAA_THRESHOLD = 7f;
    private const float WCAG_AA_THRESHOLD = 4.5f;
    private const int DEFAULT_SPACE_SIZE = 10;

    // Colors to manipulate
    private Color c1 = Color.white;
    private Color c2 = Color.black;

    // Helper styles to make it look pretty :)
    private GUIStyle previewStyle;
    private GUIStyle wcagStyle;
    private GUIStyle passStyle;
    private GUIStyle failStyle;
    private GUIStyle contrastStyle;

    [MenuItem(MENU_ITEM_NAME)]
    public static void ShowWindow()
    {
        // Set up the window and don't let it get too small
        ColorAccessibilityChecker window = (ColorAccessibilityChecker)EditorWindow.GetWindow(typeof(ColorAccessibilityChecker), false, WINDOW_NAME);
        window.minSize = new Vector2(230, 260);
        window.Show();
    }

    void OnGUI()
    {
        // Add some padding to the whole window (across the top)
        GUILayout.Space(DEFAULT_SPACE_SIZE);
        EditorGUILayout.BeginHorizontal();
            // Add some padding to the whole window (down the left)
            GUILayout.Space(DEFAULT_SPACE_SIZE);
            // Define the right column
            EditorGUILayout.BeginVertical(GUILayout.Width(210));
                // Draw the Color Pickers
                EditorGUILayout.BeginHorizontal();
                    c1 = EditorGUILayout.ColorField(new GUIContent(), c1, false, false, false, GUILayout.Width(100), GUILayout.Height(100));
                    GUILayout.Space(DEFAULT_SPACE_SIZE);
                    c2 = EditorGUILayout.ColorField(new GUIContent(), c2, false, false, false, GUILayout.Width(100), GUILayout.Height(100));
                EditorGUILayout.EndHorizontal();

                // Set up GUIStyle appropriately
                TryInitStyles();

                previewStyle.normal.textColor = new Color(c1.r, c1.g, c1.b, 1);
                previewStyle.normal.background = MakePreviewBackground(new Color(c2.r, c2.g, c2.b, 1));

                // Draw the preview
                GUILayout.Space(DEFAULT_SPACE_SIZE);
                EditorGUILayout.LabelField("Lorem Ipsum", previewStyle, GUILayout.Height(30));

                GUILayout.Space(DEFAULT_SPACE_SIZE);

                float constrast = CalculateContrast(c1, c2);

                // WCAG Labels
                EditorGUILayout.BeginHorizontal();
                    bool isAA = constrast > WCAG_AA_THRESHOLD;
                    EditorGUILayout.LabelField("WCAG AA:", wcagStyle, GUILayout.Width(100), GUILayout.Height(18));
                    EditorGUILayout.LabelField(isAA ? "Pass" : "Fail", isAA ? passStyle : failStyle, GUILayout.Width(100), GUILayout.Height(18));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    bool isAAA = constrast > WCAG_AAA_THRESHOLD;
                    EditorGUILayout.LabelField("WCAG AAA:", wcagStyle, GUILayout.Width(100), GUILayout.Height(18));
                    EditorGUILayout.LabelField(isAAA ? "Pass" : "Fail", isAAA ? passStyle : failStyle, GUILayout.Width(100), GUILayout.Height(18));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(DEFAULT_SPACE_SIZE);

                // Contrast Labels
                EditorGUILayout.LabelField("Contrast", contrastStyle, GUILayout.Height(24));
                EditorGUILayout.LabelField($"{constrast.ToString("n2")}:1", contrastStyle, GUILayout.Height(18));
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    // Calculates contrast based off of a Unity Color where R, G, B are 0-1
    private float CalculateContrast(Color c1, Color c2)
    {
        float relativeLuminancec1 = 0.2126f * GetChannelForRelativeLuminance(c1.r) + 0.7152f * GetChannelForRelativeLuminance(c1.g) + 0.0722f * GetChannelForRelativeLuminance(c1.b);
        float relativeLuminancec2 = 0.2126f * GetChannelForRelativeLuminance(c2.r) + 0.7152f * GetChannelForRelativeLuminance(c2.g) + 0.0722f * GetChannelForRelativeLuminance(c2.b);

        float l1 = Mathf.Max(relativeLuminancec1, relativeLuminancec2);
        float l2 = Mathf.Min(relativeLuminancec1, relativeLuminancec2);

        return (l1 + 0.05f) / (l2 + 0.05f);
    }

    // Makes sure the channel value is correctly calculated as per relative luminance guidelines
    private float GetChannelForRelativeLuminance(float f)
    {
        if (f < 0.03928f)
            return f / 12.92f;
        else
            return Mathf.Pow((f + 0.055f) / 1.055f, 2.4f);
    }

    // Creates all relevant styles, trying to keep OnGUI clean(ish)
    private void TryInitStyles()
    {
        if (previewStyle == null)
        {
            previewStyle = new GUIStyle(EditorStyles.boldLabel);
            previewStyle.alignment = TextAnchor.MiddleCenter;
            previewStyle.fontSize = 16; 
        }

        if (wcagStyle == null)
        {
            wcagStyle = new GUIStyle(EditorStyles.boldLabel);
            wcagStyle.fontSize = 14;
        }

        if (passStyle == null)
        {
            passStyle = new GUIStyle(wcagStyle);
            passStyle.normal.textColor = Color.green;
        }

        if (failStyle == null)
        {
            failStyle = new GUIStyle(wcagStyle);
            failStyle.normal.textColor = Color.red;
        }

        if (contrastStyle == null)
        {
            contrastStyle = new GUIStyle(EditorStyles.boldLabel);
            contrastStyle.alignment = TextAnchor.MiddleCenter;
            contrastStyle.fontSize = 16;
        }
    }

    // Creates the background texture to help with the preview
    private Texture2D MakePreviewBackground(Color color)
    {
        Color[] colors = new Color[] { color };

        Texture2D result = new Texture2D(1, 1);
        result.SetPixels(colors);
        result.Apply();

        return result;
    }
}
