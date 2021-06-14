using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject userInterface;
    [SerializeField] private GameObject controlsLeft;
    [SerializeField] private GameObject controlsRight;
    [SerializeField] private GameObject controlsUp;
    [SerializeField] private GameObject controlsDown;
    [SerializeField] private GameObject controlsAttack;

    enum ControlEventType
    {
        Up, Down, Left, Right, Attack, Invalid
    }

    private Dictionary<Vector2, string> moveAnimations = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Base Layer.Player Walk Up"},
        { Vector2.down, "Base Layer.Player Walk Down"},
        { Vector2.left, "Base Layer.Player Walk Left"},
        { Vector2.right, "Base Layer.Player Walk Right"}
    };

    private Dictionary<Vector2, string> attackAnimations = new Dictionary<Vector2, string>
    {
        { Vector2.up, "Base Layer.Attack Up"},
        { Vector2.down, "Base Layer.Attack Down"},
        { Vector2.left, "Base Layer.Attack Left"},
        { Vector2.right, "Base Layer.Attack Right"}
    };

    [SerializeField] private GameObject attackSprite;
    private Vector2 attackSpriteOffset;

    private GraphicRaycaster userInterfaceRaycaster;
    private EventSystem eventSystem;

    private Animator playerAnimator;
    private bool animationLock = false;

    private bool attackSelected = false;

    private float animationDuration = 1;

    // Start is called before the first frame update
    void Start()
    {
        userInterfaceRaycaster = userInterface.GetComponent<GraphicRaycaster>();
        eventSystem = userInterface.GetComponent<EventSystem>();
        playerAnimator = GetComponentInChildren<Animator>();

        attackSpriteOffset = attackSprite.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        PlayerAction(GetControlEvent());

    }

    private void PlayerAction(ControlEventType controlEventType)
    {
        
        //checks to see if controls are locked due to an animation being played out
        if (!animationLock)
        {

            switch (controlEventType) 
            {
                case ControlEventType.Up:
                {
                    HandleMoveEvent(Vector2.up);
                    break;
                }
                case ControlEventType.Down:
                {
                    HandleMoveEvent(Vector2.down);
                    break;
                }
                case ControlEventType.Left:
                {
                    HandleMoveEvent(Vector2.left);
                    break;
                }
                case ControlEventType.Right:
                {
                    HandleMoveEvent(Vector2.right);
                    break;
                }
                case ControlEventType.Attack:
                {
                    //switches the attack state
                    attackSelected = !attackSelected;

                    if (attackSelected)
                    {
                        //switches the ui elements to their attack state
                        SetUIAttack();
                    }
                    else
                    {
                        //switches the ui elements back to default
                        SetUIDefault();
                    }
                    break;
                }
                case ControlEventType.Invalid:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }

        }
    }


    private IEnumerator MovePlayer(Vector2 destination)
    {
        //sets starting variables for the player movement
        float timeElapsed = 0;
        Vector2 StartingPosition = transform.position;

        //loops until animation has finished
        while (timeElapsed < animationDuration)
        {
            //changes the position smoothly between the starting position and destination
            transform.position =
                new Vector2(
                    Mathf.Lerp(StartingPosition.x, destination.x, timeElapsed / animationDuration),
                    Mathf.Lerp(StartingPosition.y, destination.y, timeElapsed / animationDuration));
            //adds to the counter for time elapsed with the time between frames 
            timeElapsed += Time.deltaTime;
            //waits one frame to continue the coroutine
            yield return 0;
        }
        //sets the player to the destination after the time has elapsed to avoid potentially undershooting destination
        if (timeElapsed > animationDuration)
        {
            transform.position = destination;
        }
    }

    private IEnumerator AnimationLock()
    {
        //locks input for the duration of the animation and unlocks it after the delay
        animationLock = true;
        yield return new WaitForSeconds(animationDuration);
        animationLock = false;
    }

    private bool CheckCollision(Vector2 direction)
    {
        //sets the origin to be center of the tile the player is on
        Vector2 raycastOrigin = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.5f);
        //sends a raycast 1 time in the direction we want to check
        RaycastHit2D rayCastHit = Physics2D.Raycast(raycastOrigin, direction, 1);
        //checks to make sure the collider isn't null before using it
        if (rayCastHit.collider != null)
        {
            //checks to see if it's a collion object and returns the result
            return rayCastHit.collider.gameObject.CompareTag("Collider");
        }
        else
        {
            //returns nothing to collide with if the raycast hits nothing
            return false;
        }
    }

    private void SetUIAttack()
    {
        //switches the ui elements to their attack state
        controlsAttack.GetComponent<UIManager>().UseAttackSprite();
        controlsUp.GetComponent<UIManager>().UseAttackSprite();
        controlsDown.GetComponent<UIManager>().UseAttackSprite();
        controlsLeft.GetComponent<UIManager>().UseAttackSprite();
        controlsRight.GetComponent<UIManager>().UseAttackSprite();
    }

    private void SetUIDefault()
    {
        //switches the ui elements back to default
        controlsAttack.GetComponent<UIManager>().UseDefaultSprite();
        controlsUp.GetComponent<UIManager>().UseDefaultSprite();
        controlsDown.GetComponent<UIManager>().UseDefaultSprite();
        controlsLeft.GetComponent<UIManager>().UseDefaultSprite();
        controlsRight.GetComponent<UIManager>().UseDefaultSprite();
    }

    private IEnumerator DeactivateAttackSprite()
    {
        //sets a timer to deactive the attack sprite after the attack is finished
        yield return new WaitForSeconds(animationDuration);
        attackSprite.SetActive(false);
    }


    ControlEventType GetControlEvent()
    {
        ControlEventType controlEventType = ControlEventType.Invalid;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //sets up pointer data for the mouse to be used in raycast
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            //creats list to store raycast results
            List<RaycastResult> raycastResults = new List<RaycastResult>();

            //graphics raycast performs raycast and stores restults in raycast results
            userInterfaceRaycaster.Raycast(pointerEventData, raycastResults);

            //loops through all the objects the raycast hit
            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.Equals(controlsUp))
                {
                    controlEventType = ControlEventType.Up;
                }
                else if (result.gameObject.Equals(controlsDown))
                {
                    controlEventType = ControlEventType.Down;
                }
                else if (result.gameObject.Equals(controlsLeft))
                {
                    controlEventType = ControlEventType.Left;
                }
                else if (result.gameObject.Equals(controlsRight))
                {
                    controlEventType = ControlEventType.Right;
                }
                else if (result.gameObject.Equals(controlsAttack))
                {
                    controlEventType = ControlEventType.Attack;
                }

                if (controlEventType != ControlEventType.Invalid)
                {
                    break;
                }
            }
        }
        return controlEventType;
    }

    void HandleMoveEvent(Vector2 direction)
    {
        if (!attackSelected)
        {
            //checks if there is something to collide with in the direction of movement
            if (!CheckCollision(direction))
            {
                //sets the new destination for movement
                Vector2 destination = new Vector2(transform.position.x, transform.position.y) + direction;
                //plays the relative animation for the button
                playerAnimator.Play(moveAnimations[direction], -1, 0);
                //starts coroutine to move player based on the new destination
                StartCoroutine(MovePlayer(destination));
                //locks controls until the animation has finished
                StartCoroutine(AnimationLock());
            }
        }
        else
        {
            
            attackSprite.SetActive(true);
            //sets position of the attack sprite relative to the player
            attackSprite.transform.position = new Vector2(transform.position.x, transform.position.y) + direction + attackSpriteOffset;
            attackSprite.GetComponent<Animator>().Play(attackAnimations[direction], -1, 0);
            //actives the players attack animation
            playerAnimator.Play("Base Layer.Player Attack", -1, 0);
            StartCoroutine(DeactivateAttackSprite());
            StartCoroutine(AnimationLock());
        }
    }
}
