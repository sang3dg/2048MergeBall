using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UI_PopPanelBase : UI_PanelBase
    {
        protected override IEnumerator Show()
        {
            _CanvasGroup.alpha = 1;
            _CanvasGroup.blocksRaycasts = true;
            yield return null;
        }
        protected override IEnumerator Close()
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.blocksRaycasts = false;
            yield return null;
        }
    }
}
