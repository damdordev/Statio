namespace Damdor.Statio
{
    /// <summary>
    /// Handles an error reported by Statio.
    /// </summary>
    /// <param name="visualState">The component reporting the error, or null if no component is associated.</param>
    /// <param name="error">The error message.</param>
    public delegate void StatioErrorHandler(VisualState visualState, string error);
}