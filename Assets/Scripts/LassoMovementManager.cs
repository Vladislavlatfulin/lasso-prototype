using Services.Input;
using UnityEngine;
using Zenject;

public class LassoMovementManager: MonoBehaviour
{
    [Inject] private InputService _inputService;
    private Rope.Rope _lasso;
    private bool _startMove;
    private bool _endMove;

    private void OnEnable() =>_inputService.AddPointerMoveListener(MoveLasso);
    private void OnDisable() =>_inputService.RemovePointerMoveListener(MoveLasso);
    

    private void MoveLasso(InputPointer inputPointer)
    {
        if (inputPointer.Delta.magnitude > 40)
        {
            if (_lasso && !_startMove)
            {
                _lasso.SwipeMove(inputPointer.Delta * 0.01f);
                _startMove = true;
            }
        }
    }

    public void SetLasso(Rope.Rope lasso)
    {
        _lasso = lasso;
        _startMove = false;
    }
}