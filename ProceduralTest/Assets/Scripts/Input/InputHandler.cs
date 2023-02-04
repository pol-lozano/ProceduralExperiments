//Author: Pol Lozano Llorens
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu()]
public class InputHandler : ScriptableObject, GameInput.IGameplayActions
{
    //CAMERA
    public event UnityAction<Vector2> cameraRotateEvent = delegate { };
    public event UnityAction<float> cameraZoomEvent = delegate { };

    //PLAYER
    public event UnityAction<Vector2> moveEvent = delegate { };
    public event UnityAction jumpEvent = delegate { };

    private GameInput gameInput;

    private void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
        }
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableGameplayInput()
    {
        gameInput.Gameplay.Enable();
    }

    private void DisableAllInput()
    {
        gameInput.Gameplay.Disable();
    }

    #region GAMEPLAY
    public void OnCameraRotate(InputAction.CallbackContext context)
    {
        cameraRotateEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        cameraZoomEvent.Invoke(context.ReadValue<float>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpEvent.Invoke();
    }
    #endregion
}