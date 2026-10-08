using System;
using UnityEngine;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls CanvasGroup.interactable through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("CanvasGroup/Interactable")]
    public class CanvasGroupInteractableStatioParameter : StatioParameter<CanvasGroup, bool>
    {
        protected override bool GetValue(CanvasGroup target) => target.interactable;
        protected override void SetValue(CanvasGroup target, bool value) => target.interactable = value;
    }
}
