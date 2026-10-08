// ReSharper disable UnusedMember.Global
namespace Damdor.Statio
{
    /// <summary>
    /// Selects the delta time used for automatic animation updates.
    /// </summary>
    public enum StatioTimescale
    {
        /// <summary>
        /// Uses Time.deltaTime, affected by Time.timeScale.
        /// </summary>
        Normal,
        /// <summary>
        /// Uses Time.unscaledDeltaTime, independent of Time.timeScale.
        /// </summary>
        Unscaled
    }
}