using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using DG.Tweening;

public class DragCamera : MonoBehaviour {
    public Transform camera;
    public float minX, maxX;

    private Tweener tweener;

    // Display swipe angle
    public void OnDrag(Gesture gesture)
    {
        var deltaX = gesture.swipeVector.x;

        if (deltaX != 0f)
        {
            if (tweener != null)
            {
                tweener.Kill();
                tweener = null;
            }
            camera.Translate(Vector3.right * -deltaX / 100f);
            var posCamera = camera.localPosition;
            if (posCamera.x < minX) camera.localPosition = new Vector3(minX, 0f, 0f);
            if (posCamera.x > maxX) camera.localPosition = new Vector3(maxX, 0f, 0f);
        }
    }

    public void OnDragEnd(Gesture gesture)
    {
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
        }
        tweener = camera.DOLocalMoveX(0f, 0.5f);
    }
}
