using System.Collections;
using System.Collections.Generic;
using Drifted;
using UnityEngine;
using System.Linq;

public class Highlighter : MonoBehaviour
{
    [SerializeField]
    bool highlight = false;

    private Color[] HighlightColors = new Color[] { Color.red, new Color(.25f, .21f, 0f, 0.28f), Color.blue };
    private int CurrentHighlightColor;
    MaterialPropertyBlock propertyBlock;
    private EzTimer colorLerpTimer;

    private Renderer thisRenderer;

    public float Amplitude = 0.2f;
    public float Speed = 1.0f;

    public float Amount = 0.2f;

    private bool m_HologramMode = false;

    void Awake()
    {
        thisRenderer = GetComponentInChildren<Renderer>();
        colorLerpTimer = new EzTimer(1.0f, () =>
        {
            CurrentHighlightColor++;
            if (CurrentHighlightColor > HighlightColors.Length - 1) CurrentHighlightColor = 0;
            colorLerpTimer.Start();
        }, false);
        propertyBlock = new MaterialPropertyBlock();
    }

    void SetShader(Shader shader, bool preserveColor = true)
    {
        var renderers = GetComponentsInChildren<Renderer>();

        thisRenderer = renderers.FirstOrDefault(x => x.gameObject.layer != LayerDefinitions.DriftedMinimap);
        if(thisRenderer == null)
        {
            Debug.LogWarning("Renderer was null on " + gameObject.name + ". No changes made.", gameObject);
            return;
        }

        foreach(var mat in thisRenderer.materials)
        {
            mat.shader = shader;


            if (mat.shader.name.Contains("Hologram"))
            {
                //Debug.Log("Shader props set");
                mat.SetFloat("_Amplitude", Amplitude);
                mat.SetFloat("_Speed", Speed);
                mat.SetFloat("_Amount", Amount);
            }
            else if(mat.shader.name.Contains("Outline"))
            {
                // Boujee outline
                mat.SetColor("_FirstOutlineColor", Color.yellow);
                mat.SetFloat("_FirstOutlineWidth", 0.15f);

                mat.SetColor("_SecondOutlineColor", Color.yellow);
                mat.SetFloat("_SecondOutlineWidth", 0.15f);
            }
            //mat.SetColor("Tint Color", preservedColor);
        }
    }

    [SerializeField]
    bool currentlyHighlighted = false;

    void Update()
    {
        // TODO: Update this
        return;

        if (colorLerpTimer != null && colorLerpTimer.enabled && highlight) colorLerpTimer.Tick(Time.deltaTime);

        if (thisRenderer == null) return;

        if (highlight && currentlyHighlighted == false)
        {
            //Debug.Log("Highlight.");
            //propertyBlock.SetColor("_Color", Color.Lerp(GetCurrentColor(), GetNextColor(), colorLerpTimer.Time));
            //thisRenderer.SetPropertyBlock(propertyBlock);
            currentlyHighlighted = true;
        }
        else if (!highlight && currentlyHighlighted == true)
        {
            //(thisRenderer as MeshRenderer)
            //Debug.Log("Don't highlight.");
            //thisRenderer.SetPropertyBlock(null);
            //currentlyHighlighted = false;
        }

        /*
        if (!DriftedConstants.Instance.MenusActive && currentlyHighlighted)
        {
            Dehightlight();
            currentlyHighlighted = false;
        }
        */
    }

    private Color GetCurrentColor()
    {
        return HighlightColors[CurrentHighlightColor];
    }

    private Color GetNextColor()
    {
        int newIndex = CurrentHighlightColor + 1;
        if (newIndex > HighlightColors.Length - 1) newIndex = 0;

        return HighlightColors[newIndex];
    }

    void SetMat(Material mat, bool preserveColor = false)
    {
        var thisRenderer = GetComponent<Renderer>();
        for(int i = 0; i < thisRenderer.materials.Length; i++)
        {
            Color preservedColor = Color.white;
            if(preserveColor)
            {
                preservedColor = thisRenderer.materials[i].color;
            }

            thisRenderer.materials[i] = mat;

            thisRenderer.materials[i].color = preservedColor;
        }
    }

    void SetAllMatColor(Color color)
    {
        var thisRenderer = GetComponent<Renderer>();
        for (int i = 0; i < thisRenderer.materials.Length; i++)
        {
            thisRenderer.materials[i].color = color;
        }
    }


    public void Highlight(bool hologramMode = false)
    {
        if (thisRenderer == null) GetComponentInChildren<Renderer>();

        if(hologramMode)
        {
            //SetShader(Shader.Find("Unlit/SpecialFX/Cool Hologram"));
            SetShader(Shader.Find("Unlit/SpecialFX/Cool Hologram"), true);
            m_HologramMode = true;
        }
        else
        {
            highlight = true;
            SetShader(Shader.Find("Outlined/UltimateOutlineShadows"), true);
        }

        //MaterialPropertyBlock props = new MaterialPropertyBlock();
        //props.SetColor("_Color", new Color(1f, 0, 0, 0.25f));
        //GetComponent<Renderer>().SetPropertyBlock(props);
    }

    public void Dehightlight()
    {
        //Debug.Log("Dehighlight");
        SetShader(Shader.Find("Standard"));
        highlight = false;
    }
}
