using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GrowthMachine : MonoBehaviour
{
    // event to fire when the plant advances to the next growth stage
    public delegate void GrownToStageHandler(Object sender, int growthStage, int growthStages);
    public event GrownToStageHandler GrownToStage;

    // event to fire when the plant has completely grown
    public delegate void CompletelyGrownHandler(Object sender, int growthStage, int growthStages);
    public event CompletelyGrownHandler CompletelyGrown;

    // event to fire when the plant starts to growth
    public delegate void StartedGrowingHandler(Object sender, int growthStage, int growthStages);
    public event StartedGrowingHandler StartedGrowing;

    // event to fire when the plant stops to growth
    public delegate void StoppedGrowingHandler(Object sender, int growthStage, int growthStages);
    public event StoppedGrowingHandler StoppedGrowing;

    // Sprites for each growth stage
    [SerializeField]
    protected List<Sprite> m_stages;

    // how long to wait in seconds between each growth stage
    [SerializeField]
    [Range(0.01f, 100.0f)]
    protected float m_growthRate;

    // should I start growing the moment I am planted?
    [SerializeField]
    protected bool m_growImmediately;

    // what stage of growth is this plant at?
    private int m_growthStage;

    // cached sprite renderer component
    private SpriteRenderer m_spriteRenderer;

    // indicates whether the plant is growing or not
    private bool m_isGrowing;

    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        // should I start growing the moment I am planted?
        if (m_growImmediately)
        {
            StartGrowing();
        }

    }

    // stops the plant from growing and terminates the coroutine that makes the plant grow
    public void StopGrowing()
    {
        // am I growing? if not, do nothing
        if (m_isGrowing == false)
        {
            return;
        }

        StopCoroutine(Growing());
        m_isGrowing = false;
        FireStoppedGrowing();
    }

    // forces the plant to start growing -- either automatically when it is planted or forcibly
    public void StartGrowing()
    {
        // am I already growing? if so, do nothing
        if (m_isGrowing)
        {
            return;
        }

        StartCoroutine(Growing());
    }

    // coroutine that will grow the plant through the various stages to the end
    IEnumerator Growing()
    {
        m_isGrowing = true;
        // verify we don't have a negative or zero growth rate
        System.Diagnostics.Debug.Assert(m_growthRate > 0.0f);
        // verify we have a list of sprites to handle the growth stages
        System.Diagnostics.Debug.Assert(m_stages != null);
        int stagesToGrow = m_stages.Count;
        // verify we have at least one sprite to handle the first growth stage
        System.Diagnostics.Debug.Assert(stagesToGrow > 0);
        FireStartedGrowing();
        ResetGrowth();
        for (int i = 1; i < stagesToGrow; i++)
        {
            yield return new WaitForSeconds(m_growthRate);
            AdvanceToNextGrowthStage();
        }

        m_isGrowing = false;
        FireStoppedGrowing();
    }

    // reset the growth of the plant to the very first stage. Assumes we have a first stage to reset to
    public void ResetGrowth()
    {
        GrowthStage = 0;
    }

    // advance the plant to the next stage of growth
    public void AdvanceToNextGrowthStage()
    {
        // have we grown to the last stage? if so, do nothing more
        if (GrowthStage >= (m_stages.Count - 1))
        {
            return;
        }

        // update the sprite to the next growth stage
        GrowthStage++;
        // let any other scripts paying attention to this plant know about the growth
        FireAdvancedToNextStage();
        // let any other scripts paying attention to this plant know about reaching the final stage growth
        if (GrowthStage == (m_stages.Count - 1))
        {
            FireCompletelyGrown();
        }

    }

    // reset the script values to sane defaults
    void Reset()
    {
        m_stages = new List<Sprite>();
        m_growthRate = 1.0f;
        m_growImmediately = true;
    }

    // inform any other scripts listening to this plant that i've started growing -- either forcibly or because I
    // started automatically.
    private void FireStartedGrowing()
    {
        if (StartedGrowing != null)
        {
            StartedGrowing(this, m_growthStage, m_stages.Count);
        }

    }

    // inform any other scripts listening to this plant that i've stopped growing -- either forcibly or because I
    // reached the end of my growth cycle).
    private void FireStoppedGrowing()
    {
        if (StoppedGrowing != null)
        {
            StoppedGrowing(this, m_growthStage, m_stages.Count);
        }

    }

    // inform any other scripts listening to this plant that i've completely grown up
    private void FireCompletelyGrown()
    {
        if (CompletelyGrown != null)
        {
            CompletelyGrown(this, (int)(m_growthStage), m_stages.Count);
        }
    }

    // inform any other scripts listening to this plant that i've grown to the next stage
    private void FireAdvancedToNextStage()
    {
        if (GrownToStage != null)
        {
            GrownToStage(this, (int)(m_growthStage), m_stages.Count);
        }

    }

    // permits the growth stage to be immediately set by other scripts
    public int GrowthStage
    {
        get
        {
            return m_growthStage;
        }
        set
        {
            System.Diagnostics.Debug.Assert(value >= 0);
            System.Diagnostics.Debug.Assert(value < m_stages.Count);
            m_growthStage = value;
            m_spriteRenderer.sprite = m_stages[value];
        }
    }

    // indicates whether the plant is currently growing or not
    public bool IsGrowing
    {
        get
        {
            return m_isGrowing;
        }
    }

}