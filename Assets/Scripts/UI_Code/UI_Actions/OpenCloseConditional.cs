using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseConditional : OpenClosePanel
{
    [SerializeField] public bool openOnWin = false; // only opens on the win state of the level.
    [SerializeField] public bool openOnLose = false; // only opens on the close state of the level.

    // Update is called once per frame
    void Update()
    {
        if ((this.openOnWin || this.openOnLose) && !LevelManager.Instance.ProgressStatusLevel()){
            if (this.openOnWin && LevelManager.Instance.WinStatusLevel()){
                ForceOpenPanel();
            } else if (this.openOnLose && !LevelManager.Instance.WinStatusLevel()){
                ForceOpenPanel();
            }
        }
    }
}
