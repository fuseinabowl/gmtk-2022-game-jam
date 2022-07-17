using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JackProtractorController : MonoBehaviour
{
    [SerializeField]
    private Canvas protractorCanvas = null;
    [SerializeField]
    private TMP_Text leftMovesText = null;
    [SerializeField]
    private TMP_Text upLeftMovesText = null;
    [SerializeField]
    private TMP_Text upMovesText = null;
    [SerializeField]
    private TMP_Text upRightMovesText = null;
    [SerializeField]
    private TMP_Text rightMovesText = null;
    [SerializeField]
    private TMP_Text wildMovesText = null;
    
    [SerializeField]
    private ConsumableMovements consumeableMovements = null;

    [SerializeField]
    private Color greenColor = new Color( 0, 255, 0, 255);
    [SerializeField]
    private Color orangeColor = new Color( 255, 204, 0, 255);
    [SerializeField]
    private Color redColor = new Color( 255, 0, 0, 255);

    // Start is called before the first frame update
    void Start()
    {
        protractorCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        int leftMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.Left);
        int upLeftMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.LeftUp);
        int upMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.Up);
        int upRightMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.RightUp);
        int rightMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.Right);
        int wildMovements = consumeableMovements.GetAvailableMovementActions(ConsumableMovements.Movement.Stop);

        bool hasWildMoves = wildMovements > 0;
        leftMovesText.text = leftMovements.ToString();
        leftMovesText.color = leftMovements > 0 ? greenColor : hasWildMoves ? orangeColor : redColor;

        upLeftMovesText.text = upLeftMovements.ToString();
        upLeftMovesText.color = upLeftMovements > 0 ? greenColor : hasWildMoves ? orangeColor : redColor;

        upMovesText.text = upMovements.ToString();
        upMovesText.color = upMovements > 0 ? greenColor : hasWildMoves ? orangeColor : redColor;

        upRightMovesText.text = upRightMovements.ToString();
        upRightMovesText.color = upRightMovements > 0 ? greenColor : hasWildMoves ? orangeColor : redColor;

        rightMovesText.text = rightMovements.ToString();
        rightMovesText.color = rightMovements > 0 ? greenColor : hasWildMoves ? orangeColor : redColor;

        wildMovesText.text = wildMovements.ToString();
        wildMovesText.color = hasWildMoves ? greenColor : redColor;
    }

    public void SetMovesToBeUsed( bool moveSet, ConsumableMovements.Movement movement, bool usingWild )
    {
        leftMovesText.fontStyle = FontStyles.Normal;
        upLeftMovesText.fontStyle = FontStyles.Normal;
        upMovesText.fontStyle = FontStyles.Normal;
        upRightMovesText.fontStyle = FontStyles.Normal;
        rightMovesText.fontStyle = FontStyles.Normal;
        wildMovesText.fontStyle = FontStyles.Normal;

        if (moveSet)
        {
            switch (movement)
            {
                case ConsumableMovements.Movement.Left:
                    leftMovesText.fontStyle = FontStyles.Bold;
                    break;
                case ConsumableMovements.Movement.LeftUp:
                    upLeftMovesText.fontStyle = FontStyles.Bold;
                    break;
                case ConsumableMovements.Movement.Up:
                    upMovesText.fontStyle = FontStyles.Bold;
                    break;
                case ConsumableMovements.Movement.RightUp:
                    upRightMovesText.fontStyle = FontStyles.Bold;
                    break;
                case ConsumableMovements.Movement.Right:
                    rightMovesText.fontStyle = FontStyles.Bold;
                    break;
            }
            if (usingWild)
            {
                wildMovesText.fontStyle = FontStyles.Bold;
            }
        }
    }
}
