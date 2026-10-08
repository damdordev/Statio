using System;
using UnityEngine;

namespace Damdor.Statio
{
    [Serializable]
    [StatioParameter("CanvasGroup/Blocks Raycasts")]
    public class CanvasGroupBlocksRaycastsStatioParameter : StatioParameter<CanvasGroup, bool>
    {
        protected override bool GetValue(CanvasGroup target) => target.blocksRaycasts;
        protected override void SetValue(CanvasGroup target, bool value) => target.blocksRaycasts = value;
    }
}
