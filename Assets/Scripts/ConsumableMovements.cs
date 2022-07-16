using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ConsumableMovements : MonoBehaviour
{
    private bool isReadyToShare = false;

    private int[] availableMovements = new int[6];

    public void SetAvailableMovements(
        int up,
        int left,
        int leftUp,
        int rightUp,
        int right,
        int stop
    )
    {
        Assert.IsFalse(isReadyToShare);

        availableMovements[0] = up;
        availableMovements[1] = left;
        availableMovements[2] = leftUp;
        availableMovements[3] = rightUp;
        availableMovements[4] = right;
        availableMovements[5] = stop;

        isReadyToShare = true;
    }

    public enum Movement
    {
        Up,
        Left,
        LeftUp,
        RightUp,
        Right,
        Stop,
    }

    public int GetAvailableMovementActions(Movement movement)
    {
        if (isReadyToShare)
        {
            return availableMovements[(int)movement];
        }
        else
        {
            return 0;
        }
    }

    public void ConsumeMovement(Movement movement)
    {
        var movementIndex = (int)movement;
        var numberOfTargetMovementAvailable = availableMovements[movementIndex];
        Assert.IsTrue(numberOfTargetMovementAvailable > 0);
        availableMovements[movementIndex] = numberOfTargetMovementAvailable - 1;
    }

    public void StopSharingMovements()
    {
        isReadyToShare = false;
        for (var moveIndex = 0; moveIndex < availableMovements.Length; ++moveIndex)
        {
            availableMovements[moveIndex] = 0;
        }
    }
}
