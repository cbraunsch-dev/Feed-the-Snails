using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Snail : MonoBehaviour
{
    Animator animator;
    public bool Dead { get; private set; }
    public GameColor gameColor;
    public bool king = false;
    public GameObject slime;    // A segment of the slime trail that appears behind a snail as it moves
    public GameObject slimePuddle;  // The puddle of slime that appears when a snail has died
    public GameObject avatar;

    [Header("Speed at which indicator disc scales up and down when snail enters/exits reticle")]
    public float indicatorDiscScaleSpeed;
    private float timeSinceLastSlimeSegmentSpawn = 0.0f;
    private const float delayBeforeSpawningSlimeSegment = 0.25f;
    private Vector3? locationOfNextSlimeSegment;
    private Vector3? directionOfNextSlimeSegment;
    private bool shouldSpawnSlime = false;
    private Transform indicatorDisc;
    private Vector3 minIndicatorDiscScale = Vector3.one;
    private Vector3 maxIndicatorDiscScale = Vector3.one * 1.5f;
    private TweenerCore<Color, Color, ColorOptions> indicatorDiscPulsatingTweener;
    private bool pulsatingIndicatorDisc = false;
    private Color pulsatingIndicatorDiscColor;

    public GameObject SelectionIndicator { get
        {
            return transform.Find("SelectionIndicator").gameObject;
        }
    }
    public SnailListener SnailListener { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        indicatorDisc = transform.Find("IndicatorDisc");
        animator = gameObject.GetComponentInChildren<Animator>();
        var primaryPlayerColor = gameColor.PrimaryColorRepresentation();
        var secondaryPlayerColor = gameColor.SecondaryColorRepresentation();
        var tertiaryPlayerColor = gameColor.TertiaryColorRepresentation();
        SetSnailColor(primaryPlayerColor, secondaryPlayerColor, tertiaryPlayerColor);
        SetColorIndicatorToInitialState();
        HideCrownForNonKings();
    }

    private void Update()
    {
        if (shouldSpawnSlime)
        {
            timeSinceLastSlimeSegmentSpawn += Time.deltaTime;
            if (timeSinceLastSlimeSegmentSpawn >= delayBeforeSpawningSlimeSegment)
            {
                var slimeInstance = Instantiate(slime, (UnityEngine.Vector3)locationOfNextSlimeSegment, Quaternion.identity);
                slimeInstance.GetComponent<Slime>().GameColor = this.gameColor;
                slimeInstance.transform.forward = (UnityEngine.Vector3)directionOfNextSlimeSegment;
                PrepareForNextSlimeSegment();
            }
        }
        if(pulsatingIndicatorDisc)
        {
            PulsateIndicatorDiscColor();
        }
    }

    private void PulsateIndicatorDiscColor()
    {
        var colorDisc = indicatorDisc.Find("ColorDisc");
        colorDisc.GetComponent<Renderer>().materials[0].SetColor("_BaseColor", pulsatingIndicatorDiscColor);   
    }

    private void SetColorIndicatorToInitialState()
    {
        var primaryPlayerColor = gameColor.PrimaryColorRepresentation();
        var tertiaryPlayerColor = gameColor.TertiaryColorRepresentation();
        var quaternaryPlayerColor = gameColor.QuaternaryColorRepresentation();
        UpdateColorIndicatorWithColors(quaternaryPlayerColor.Value, primaryPlayerColor.Value, tertiaryPlayerColor.Value);
    }

    public void ChangeColorIndicatorToSelectedState()
    {
        var primaryPlayerColor = gameColor.PrimaryColorRepresentation();
        var tertiaryPlayerColor = gameColor.TertiaryColorRepresentation();
        UpdateColorIndicatorWithColors(tertiaryPlayerColor.Value, primaryPlayerColor.Value, primaryPlayerColor.Value);
    }

    public void ChangeColorIndicatorToUnselectedState()
    {
        var invisibleColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        var tertiaryPlayerColor = gameColor.TertiaryColorRepresentation();
        var quaternaryPlayerColor = gameColor.QuaternaryColorRepresentation();
        UpdateColorIndicatorWithColors(quaternaryPlayerColor.Value, invisibleColor, tertiaryPlayerColor.Value);
    }

    private void UpdateColorIndicatorWithColors(Color discColor, Color triangleColor, Color ringColor)
    {    
        var colorDisc = indicatorDisc.Find("ColorDisc");
        var indicatorTriangle = indicatorDisc.Find("IndicatorTriangle");
        var ring = indicatorDisc.Find("Ring");
        colorDisc.GetComponent<Renderer>().materials[0].SetColor("_BaseColor", discColor);
        indicatorTriangle.GetComponent<Renderer>().materials[0].SetColor("_BaseColor", triangleColor);
        ring.GetComponent<Renderer>().materials[0].SetColor("_BaseColor", ringColor);
    }

    public void StartSliming()
    {
        PrepareForNextSlimeSegment();
        shouldSpawnSlime = true;
    }

    public void StopSliming()
    {
        shouldSpawnSlime = false;
    }

    private void PrepareForNextSlimeSegment()
    {
        timeSinceLastSlimeSegmentSpawn = 0.0f;
        var slimeY = this.transform.position.y - (this.GetComponent<BoxCollider>().size.y / 6);
        locationOfNextSlimeSegment = new Vector3(this.transform.position.x, slimeY, this.transform.position.z);
        directionOfNextSlimeSegment = this.transform.forward;
    }

    void SetSnailColor(Color? primaryPlayerColor, Color? secondaryPlayerColor, Color? tertiaryPlayerColor)
    {
        if (primaryPlayerColor != null && secondaryPlayerColor != null)
        {
            var snail = transform.Find("Snail");
            var snailBody = snail.transform.Find("Body").gameObject;
            var snailBodyRenderer = snailBody.GetComponent<Renderer>();
            snailBodyRenderer.materials[0].SetColor("_BaseColor", primaryPlayerColor.Value);
            var snailShell = snail.transform.Find("Shell").gameObject;
            var snailShellRenderer = snailShell.GetComponent<Renderer>();
            snailShellRenderer.materials[1].SetColor("_BaseColor", secondaryPlayerColor.Value);
        }
    }

    void HideCrownForNonKings()
    {
        if(!this.king)
        {
            var snail = transform.Find("Snail");
            var crown = snail.transform.Find("Crown").gameObject;
            crown.SetActive(false);
        }
    }

    public void TurnToNewDirection(MovementDirection direction)
    {
        switch(direction)
        {
            case MovementDirection.Left:
                gameObject.transform.forward = new Vector3(-1, 0, 0);
                break;
            case MovementDirection.Right:
                gameObject.transform.forward = new Vector3(1, 0, 0);
                break;
            case MovementDirection.Up:
                gameObject.transform.forward = new Vector3(0, 0, 1);
                break;
            case MovementDirection.Down:
                gameObject.transform.forward = new Vector3(0, 0, -1);
                break;
        }
    }

    public void KillSnail()
    {
        //Making a state machine transition outside of the state machine behavior scripts is usually not allowed because the triggers are sticky
        //and will remain flagged to true even if we're currently not in a state that is affected by this trigger. This would lead to the problem that
        //once the state machine moves to a state with said trigger, it will immediately change state again because the trigger is still flagged to true.
        //However, since the snail can die in every one of its states (except the initial one but that does not matter), we can set the trigger here. Furthermore
        //we have to change the state here since this is the only place where we can detect the collision. We cannot detect the collision in the state machine
        //behavior, AFAIK.
        animator.SetTrigger(SnailTrigger.Die);
    }

    public void SnailDying()
    {
        this.GetComponent<AudioSource>().Play();
        this.SnailListener.SnailDying(this);
    }

    public void SnailDidFinishDying()
    {
        this.Dead = true;
        this.SnailListener.SnailDied(this);
        this.indicatorDisc.gameObject.SetActive(false);

        // Spawn slime puddle. Rotate it a bit to make the puddles look more unique
        var targetRotation = Quaternion.Euler(new Vector3(transform.rotation.x, Random.Range(0.0f, 30.0f), transform.rotation.y));
        var slimeInstance = Instantiate(slimePuddle, transform.position, targetRotation);
        slimeInstance.GetComponent<SlimePuddle>().GameColor = this.gameColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnAvatarCollidedWith(other);
    }

    private bool CollidedWithObstacle(Collider obstacleCollider, string tagToCheckFor)
    {
        return obstacleCollider.tag == tagToCheckFor;
    }

    bool CollidedWithSaladOfWrongColor(Collider other)
    {
        return CollidedWithObstacle(other, Tags.Salad) && other.gameObject.GetComponent<Salad>().gameColor != this.gameColor;
    }

    private void OnAvatarCollidedWith(Collider other)
    {
        if (!this.Dead &&
            (CollidedWithSaladOfWrongColor(other) || CollidedWithObstacle(other, Tags.DeadlyObstacle)))
        {
            KillSnail();
        }
        if (!this.Dead &&
            (CollidedWithObstacle(other, Tags.Player)))
        {
            var collidedSnail = other.gameObject.GetComponentInParent<Snail>();
            var snailColor = collidedSnail.gameColor;
            if(this.gameColor != snailColor)
            {
                KillSnail();
                other.gameObject.GetComponentInParent<Snail>().KillSnail();
            }
        }
        if (!this.Dead &&
            (CollidedWithObstacle(other, Tags.Slime)))
        {
            var collidedSlime = other.gameObject.GetComponent<Slime>();
            var slimeColor = collidedSlime.GameColor;
            if (this.gameColor != slimeColor)
            {
                KillSnail();
            }
        }
        if(!this.Dead &&
            (CollidedWithObstacle(other, Tags.SlimePuddle)))
        {
            var collidedSlime = other.gameObject.GetComponent<SlimePuddle>();
            var slimeColor = collidedSlime.GameColor;
            if (this.gameColor != slimeColor)
            {
                KillSnail();
            }
        }
        if (CollidedWithObstacle(other, Tags.Obstacle))
        {
            gameObject.transform.forward = gameObject.transform.forward * -1;
        }
        if (CollidedWithObstacle(other, Tags.Arrow))
        {
            gameObject.transform.forward = other.gameObject.transform.forward;
        }
        if (CollidedWithObstacle(other, Tags.Portal))
        {
            var linkedPortal = other.gameObject.GetComponent<Portal>().linkedPortal;
            if (linkedPortal != null)
            {
                gameObject.transform.position = linkedPortal.transform.position;
                gameObject.transform.forward = linkedPortal.transform.forward;
            }
        }
    }

    public void OnPause()
    {

    }

    public void ScaleIndicatorDiscUp()
    {
        indicatorDisc.DOScale(maxIndicatorDiscScale, indicatorDiscScaleSpeed);
    }

    public void ScaleIndicatorDiscDown()
    {
        indicatorDisc.DOScale(minIndicatorDiscScale, indicatorDiscScaleSpeed);
    }

    public void StartPulsatingSelectionIndicator()
    {
        pulsatingIndicatorDisc = true;
        var colorDisc = indicatorDisc.Find("ColorDisc");
        pulsatingIndicatorDiscColor = colorDisc.GetComponent<Renderer>().materials[0].GetColor("_BaseColor");
        var fadedOutColor = new Color(pulsatingIndicatorDiscColor.r, pulsatingIndicatorDiscColor.g, pulsatingIndicatorDiscColor.b, 0.2f);
        indicatorDiscPulsatingTweener = DOTween.To(() => pulsatingIndicatorDiscColor, x => pulsatingIndicatorDiscColor = x, fadedOutColor, 0.5f)
            .SetLoops(-1, LoopType.Yoyo);
        
    }

    public void StopPulsatingSelectionIndicator()
    {
        indicatorDiscPulsatingTweener.Kill();
        pulsatingIndicatorDisc = false;
    }
}

public interface SnailListener
{
    void SnailDying(Snail snail);

    void SnailDied(Snail snail);
}
