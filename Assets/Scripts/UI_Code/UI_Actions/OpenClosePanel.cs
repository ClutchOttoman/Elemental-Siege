using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClosePanel : MonoBehaviour
{
    [SerializeField] public GameObject panelObject; // UI Panel Object.
    [SerializeField] public bool activePanelState = false; // indicates if the OpenClosePanel object is active or not.
    [SerializeField] public float uiMarginTop = 0.0f; // when opened, adjusts the rectangular margin of the UI bounds for the player.
    [SerializeField] public float uiMarginBottom = 0.0f;
    [SerializeField] public float uiMarginLeft = 0.0f;
    [SerializeField] public float uiMarginRight = 0.0f;

    protected void SetPlayerMenuStatus(bool isInMenu)
    {
        Debug.Log("In Menu: " + isInMenu);
        if (PlayerController.Instance != null) PlayerController.Instance.isInMenu = isInMenu;
    }

    public void ForceOpenPanel()
    {
        this.SetPlayerMenuStatus(true);
        this.activePanelState = true;
        this.panelObject.SetActive(true);
        // Debug.Log("PlayerController.Instance is " + PlayerController.Instance + ".");
        PlayerController.Instance.adjustUIBounds(uiMarginTop, uiMarginBottom, uiMarginLeft, uiMarginRight);
    }

    public void ForceClosePanel()
    {
        this.SetPlayerMenuStatus(false);
        this.activePanelState = false;
        this.panelObject.SetActive(false);
        // Debug.Log("PlayerController.Instance is " + PlayerController.Instance + ".");
        PlayerController.Instance.restoreAllDefaultBounds();
    }

    public void ForceClosePanelNoRestoreUIBounds()
    {
        this.SetPlayerMenuStatus(false);
        this.activePanelState = false;
        this.panelObject.SetActive(false);
    }
    
    public void ForceOpenPanelNoUIBounds(){
        this.activePanelState = true;
        this.panelObject.SetActive(true);
    }
    
    public void TogglePanel(){
        if (this.activePanelState == true){
            this.SetPlayerMenuStatus(false);
            this.panelObject.SetActive(false);
        } else
        {
            this.SetPlayerMenuStatus(true);
            this.panelObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.panelObject.SetActive(activePanelState);
    }

    // // Update is called once per frame
    // protected virtual void  Update()
    // {

    // }
}
