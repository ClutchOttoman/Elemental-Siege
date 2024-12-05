using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance { get; private set; }

    private static Dictionary<string, PlayerRespawnData> RESPAWN_DATA = new Dictionary<string, PlayerRespawnData>
    {
        { "Level1", new PlayerRespawnData(BoundsUtil.FromAABB(new Vector3(14, 20, -22), new Vector3(-36, 0, 28)), new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0f)) },
        { "Level2", new PlayerRespawnData(BoundsUtil.FromAABB(new Vector3(14, 20, -22), new Vector3(-36, 0, 28)), new Vector3(0, 10, 0), Quaternion.Euler(0, 0, 0f)) }
    };

    public float moveSpeed = 5f; // Speed of movement
    public float mouseSensitivity = 100f; // Sensitivity of mouse rotation
    private float rotationX = 0f; // For vertical mouse rotation
    private float moveBackToBoundsMovementFactor = 0.5f; // How fast to move back to bounds when player is out of bounds.

    private CharacterController controller;
    private Vector3 moveDirection;
    public bool usingDefaultPanelUISet = true; // used to indicate to use default ui settings or not.
    public bool isInMenu = false;

    // These fields store the default margins.
    [SerializeField] public float defaultTopMarginUiPercent = 0.0f; // percentage of the screen.
    [SerializeField] public float defaultBottomMarginUiPercent = 0.0f; // percentage of the screen.
    [SerializeField] public float defaultLeftMarginUiPercent = 0.0f; // percentage of the screen.
    [SerializeField] public float defaultRightMarginUiPercent = 0.0f; // percentage of the screen.

    // Whether the player controller is only there for the UI.

    [SerializeField] public bool uiOnly = false;
    // bool currPlaceTower = false; // indicates if the player is in tower placement mode. If so, the camera should not move.

    // These fields store updated and temporary UI bounds for on-screen UI elements.
    private float adjustTopMarginUiPercent = 0.0f; // percentage of the screen.
    private float adjustBottomMarginUiPercent = 0.0f; // percentage of the screen.
    private float adjustLeftMarginUiPercent = 0.0f; // percentage of the screen.
    private float adjustRightMarginUiPercent = 0.0f; // percentage of the screen.

    // Updates the temporary bounds for the UI based on more specfied parameters.
    // If one of the parameters is less than zero, than the adjusted parameter will do nothing. For that parameter, the actual UI will use the default UI.
    public void adjustUIBounds(float newTopMarginUiPercent, float newBottomMarginUiPercent, float newLeftMarginUiPercent, float newRightMarginUiPercent)
    {

        float screenWidth = (float)Screen.width;
        float screenHeight = (float)Screen.height;

        if (newTopMarginUiPercent.CompareTo(0.0f) >= 0)
        {
            this.adjustTopMarginUiPercent = newTopMarginUiPercent;
        }
        else
        {
            this.adjustTopMarginUiPercent = this.defaultTopMarginUiPercent;
        }

        if (newBottomMarginUiPercent.CompareTo(0.0f) >= 0)
        {
            this.adjustBottomMarginUiPercent = newBottomMarginUiPercent;
        }
        else
        {
            this.adjustBottomMarginUiPercent = this.defaultBottomMarginUiPercent;
        }

        if (newLeftMarginUiPercent.CompareTo(0.0f) >= 0)
        {
            this.adjustLeftMarginUiPercent = newLeftMarginUiPercent;
        }
        else
        {
            this.adjustLeftMarginUiPercent = this.defaultLeftMarginUiPercent;
        }

        if (newRightMarginUiPercent.CompareTo(0.0f) >= 0)
        {
            this.adjustRightMarginUiPercent = newRightMarginUiPercent;
        }
        else
        {
            this.adjustRightMarginUiPercent = this.defaultRightMarginUiPercent;
        }

        this.usingDefaultPanelUISet = false;
    }

    // Makes the UI see the default bounds and resets the adjusted bounds.
    public void restoreAllDefaultBounds()
    {
        // Reset adjusted settings.
        this.adjustTopMarginUiPercent = 0.0f;
        this.adjustBottomMarginUiPercent = 0.0f;
        this.adjustLeftMarginUiPercent = 0.0f;
        this.adjustRightMarginUiPercent = 0.0f;

        // Use the default UI settings.
        this.usingDefaultPanelUISet = true;
    }

    // public void setTowerPlacementMode(bool isOn){

    //     this.currPlaceTower = isOn;
    // }

    // Returns true if the mouse is within the UI bounds; false otherwise.
    public bool checkInUIBounds()
    {

        Vector3 mouseScreenPosition = Input.mousePosition;
        float screenWidth = (float)Screen.width;
        float screenHeight = (float)Screen.height;

        float actualRightMargin = 0.0f;
        float actualLeftMargin = 0.0f;
        float actualTopMargin = 0.0f;
        float actualBottomMargin = 0.0f;

        // Determines the UI. 
        if (this.usingDefaultPanelUISet)
        {
            // Use the default UI settings.
            actualRightMargin = (float)(this.defaultRightMarginUiPercent / 100.0) * screenWidth;
            actualLeftMargin = (float)(this.defaultLeftMarginUiPercent / 100.0) * screenWidth;
            actualTopMargin = (float)(this.defaultTopMarginUiPercent / 100.0) * screenHeight;
            actualBottomMargin = (float)(this.defaultBottomMarginUiPercent / 100.0) * screenHeight;

        }
        else
        {
            // Use the adjusted UI settings.
            actualRightMargin = (float)(this.adjustRightMarginUiPercent / 100.0) * screenWidth;
            actualLeftMargin = (float)(this.adjustLeftMarginUiPercent / 100.0) * screenWidth;
            actualTopMargin = (float)(this.adjustTopMarginUiPercent / 100.0) * screenHeight;
            actualBottomMargin = (float)(this.adjustBottomMarginUiPercent / 100.0) * screenHeight;
        }

        // Checks the left and right sides of the bounds.
        if (mouseScreenPosition.x.CompareTo(screenWidth - actualRightMargin) > 0 || mouseScreenPosition.x.CompareTo(actualLeftMargin) < 0)
        {
            return true;
        }

        // Checks the top and bottom sides of the bounds.
        if (mouseScreenPosition.y.CompareTo(screenHeight - actualTopMargin) > 0 || mouseScreenPosition.y.CompareTo(actualBottomMargin) < 0)
        {
            return true;
        }

        return false;

    }

    void Awake()
    {
        if (PlayerController.Instance != null && PlayerController.Instance != this)
        {
            Destroy(this);
        }
        else
        {
            PlayerController.Instance = this;
        }

    }

    void Start()
    {
        if (!this.uiOnly)
        {
            controller = GetComponent<CharacterController>(); // Get the CharacterController
        }
    }

    void Update()
    {
        if (!this.uiOnly)
        {
            // Only let the mouse dicate where the player looks with the camera when it's not in the bounds of the UI or in tower placement mode.
            if (checkInUIBounds() == false)
            {
                HandleRotation();
                HandleMovementInput();
            }

            // Do not let the player move the camera when in tower placement mode.
            CheckPlayerPosition();
        }
    }

    // Handle mouse rotation
    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Apply horizontal rotation (Y axis)
        float a = Mathf.Abs(this.rotationX / 85.0f);
        float scale = 1 / (16 * a * a * a * a + 1);
        //Debug.Log("Is in menu? " + this.isInMenu);
        if (this.isInMenu)
        {
            scale = 0;
        }
        //Debug.Log("Rotation " + this.rotationX + " and scale " + scale);
        transform.Rotate(Vector3.up * mouseX * scale);

        // Apply vertical rotation (X axis)
        if (!this.isInMenu)
        {
            this.rotationX -= mouseY;
        }
        this.rotationX = Mathf.Clamp(rotationX, -85f, 85f); // Clamping to avoid flipping

        // Apply the clamped vertical rotation
        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, 0f);
    }

    // Handle movement input
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveY = Input.GetAxis("Depth");   // Space/Shift
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Handle horizontal movement
        Vector3 horizontalMovement = transform.right * moveX + transform.forward * moveZ;
        horizontalMovement.y = 0;
        horizontalMovement.Normalize();
        horizontalMovement *= moveSpeed;

        // Handle vertical movement
        Vector3 verticalMovement = Vector3.up * moveY * moveSpeed;

        // Combine both movements
        moveDirection = horizontalMovement + verticalMovement;

        // Move the player using the CharacterController for all movement
        controller.Move(moveDirection * Time.deltaTime * moveSpeed);
    }

    // Check's the player's position, and teleports them back into bounds
    private void CheckPlayerPosition()
    {
        // Get the current scene's id
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Get the respawn data
        // PlayerRespawnData respawnData = RESPAWN_DATA[scene];
        // if (respawnData == null) return;
        // Updated 10/9/2024
        if (!RESPAWN_DATA.ContainsKey(scene)) return;
        PlayerRespawnData respawnData = RESPAWN_DATA[scene];

        // Check if the player's out of bounds
        if (respawnData.bounds.Contains(transform.position)) return;

        // They are, so teleport them to the spawn point
        transform.position = Vector3.MoveTowards(transform.position, respawnData.spawnPoint, moveBackToBoundsMovementFactor);

        Quaternion targetRotation = Quaternion.LookRotation(respawnData.bounds.center - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1);
    }
}