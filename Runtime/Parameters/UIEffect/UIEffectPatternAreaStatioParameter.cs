#if DAMDOR_STATIO_UIEFFECT
using System;
using Coffee.UIEffects;

namespace Damdor.Statio
{
    /// <summary>
    /// Controls UIEffect.patternArea through visual state values.
    /// </summary>
    [Serializable]
    [StatioParameter("UIEffect/PatternArea")]
    public class UIEffectPatternAreaStatioParameter : StatioParameter<UIEffect, PatternArea>
    {
        protected override PatternArea GetValue(UIEffect target) => target.patternArea;
        protected override void SetValue(UIEffect target, PatternArea value) => target.patternArea = value;
    }
}
#endif