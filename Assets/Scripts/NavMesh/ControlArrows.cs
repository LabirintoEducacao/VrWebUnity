using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;
using UnityEngine.AI;

public class ControlArrows : MonoBehaviour {
    public SetTarget targetUp;
    public SetTarget targetRight;
    public SetTarget targetDown;
    public SetTarget targetLeft;

    SetTarget[ ] getTargets {
        get {
            return new SetTarget[ ] { targetUp, targetRight, targetDown, targetLeft };
        }
    }

    void Start ( ) {
        changeState (false);

        foreach (SetTarget targ in getTargets) {
            targ.onTargetSelected += onTargetSelected;
        }
    }

    private void OnDestroy() {

        foreach (SetTarget targ in getTargets) {
            if (targ != null)
                targ.onTargetSelected -= onTargetSelected;
        }
    }

    void onTargetSelected(SetTarget target, Transform destination) {
        changeState(false);
    }

    /** Liga as setas que tem destino */
    public void showArrows() {
        foreach (SetTarget target in getTargets)
        {
            if (target.Target != null) {
                target.gameObject.SetActive(true);
            }
        }
    }

    public void changeState (bool active) {
        foreach (SetTarget target in getTargets) {
            target.gameObject.SetActive (active);
            if (!active) {
                target.Target = null;
            }
        }
    }

    /** define um destino para uma direção */
    public void setGoal(int direction, Transform goal) {
        switch (direction) {
            case Constants.DIRECTION_UP:
                targetUp.Target = goal;
                break;
            case Constants.DIRECTION_RIGHT:
                targetRight.Target = goal;
                break;
            case Constants.DIRECTION_DOWN:
                targetDown.Target = goal;
                break;
            case Constants.DIRECTION_LEFT:
                targetLeft.Target = goal;
                break;
            default:
                break;
        }
    }

}
