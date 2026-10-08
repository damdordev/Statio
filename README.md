# Statio

A powerful state management system for Unity that allows you to define, transition, and animate between different visual configurations of your GameObjects.

## Table of Contents

*   [Features](#features)
*   [Installation](#installation)
*   [Basic Usage](#basic-usage)
    *   [1. Setting up a Visual State Component](#1-setting-up-a-visual-state-component)
    *   [2. Using StatioButton](#2-using-statiobutton)
    *   [3. Controlling States from Code](#3-controlling-states-from-code)
    *   [4. Configuring Animations](#4-configuring-animations)
*   [Creating Custom Parameters](#creating-custom-parameters)
*   [Dependencies](#dependencies)

## Features

*   **State-Based Configuration**: Define multiple visual states (e.g., "Normal", "Hover", "Pressed", "Disabled") for a single GameObject.
*   **Parameter Control**: Control various component properties (Color, Position, Scale, Alpha, etc.) per state.
*   **Animated Transitions**: Smoothly interpolate between states with configurable duration and easing curves.
*   **Variable Storage Integration**: Use `VarioStorage` and `VarioValue<T>` to drive state values and transition parameters dynamically.
*   **Optional UIEffect Integration**: Control Coffee UIEffect properties when `com.coffee.ui-effect` is installed.
*   **Extensible**: Create custom parameters to control any property of any component.

## Installation

This package is currently under development. In the future, it will be available via a UPM registry. For now, you can install it using the Git URL.

**Option A: Install via Package Manager window**
1. In Unity, open **Window** > **Package Manager**.
2. Click the **+** button and select **Add package from git URL...**
3. Enter the following URL and click **Add**:
   `https://github.com/damdordev/Statio.git#1.0.0-preview`

**Option B: Install via `manifest.json`**
Open your project's `Packages/manifest.json` file and add the following line to your `"dependencies"` block:
```json
"com.damdor.vario": "https://github.com/damdordev/Statio.git#1.0.0-preview"
```
## Basic Usage

### 1. Setting up a Visual State Component

Add the `VisualState` component to a GameObject.

1.  **Define States**: In the Inspector, add state names to the "States" list (e.g., "Idle", "Active").
2.  **Add Parameters**: Add parameters to control specific properties. For example, a `SpriteRendererColorStatioParameter` (`SpriteRenderer/Color`) to change a SpriteRenderer's color. Assign the target object for each parameter.
3.  **Configure Values**:
    *   Set a **Default Value** for the parameter.
    *   Add overrides for specific states where the value should differ from the default.
4.  **Select the Initial State**: It is applied without animation in `Awake`. Leaving it unset keeps existing component values until a state is requested.
5.  **Assign Storage if Needed**: Targets and values can be raw values or Vario expressions evaluated using the component's `VarioStorage`.

Built-in parameters cover Transform position, Euler angles, local scale and parent; RectTransform anchors, pivot, anchored position, size delta, width and height; Graphic color, alpha and raycast target; Image sprite and fill amount; SpriteRenderer color and sprite; CanvasGroup alpha, interactability and raycasts; LayoutElement preferred dimensions; Behaviour enabled state; GameObject activity; and another `VisualState`'s state. Position and Euler angles use local space by default and can be configured for world space.

### 2. Using StatioButton

`StatioButton` is a replacement for the default Unity UI `Button`. `StatioButton` uses a `VisualState` component for visual changes and also calls the base Unity button transition. This provides you with the flexibility to animate any parameter (color, position, scale, alpha, etc.) when the button state changes.

1. Add a `StatioButton` component to a UI GameObject under a Canvas (can be added via `Component -> UI -> Statio Button`).
2. Add a `VisualState` component on the same or another GameObject.
3. In the `VisualState`, define states such as `normal`, `pressed`, and `disabled`.
4. In the `StatioButton` inspector, assign the `VisualState` component and map the states for Normal, Disabled, and Pressed interactions.
5. Add parameters in the `VisualState` to define how the button looks in each of those states.

Highlighted and Selected interactions use the Normal mapping. Empty mappings are ignored. Unity's instant transitions call `ChangeStateImmediately`; other transitions call `ChangeState`. The button retains Unity's click events, navigation, and interactability.

### 3. Controlling States from Code

You can change states using the `VisualState` API.

```csharp
using Damdor.Statio;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VisualState visualState;

    public void OnPointerEnter()
    {
        // Transition to "Hover" using a configured animation rule
        visualState.ChangeState("Hover");
    }

    public void OnPointerExit()
    {
        // Transition back to "Normal" using a configured animation rule
        visualState.ChangeState("Normal");
    }

    public void OnClick()
    {
        // Instantly snap to "Pressed" state without animation
        visualState.ChangeStateImmediately("Pressed");
    }
}
```

State names are case-sensitive. Invalid state requests report an error and leave the current transition unchanged. `CurrentState` reports the destination as soon as an active component accepts a request, even while values are still animating. Requests made while the component is inactive or disabled are deferred; only the latest request is applied on enable.

Use `AddState`, `RemoveState`, and `ChangeStateId` to manage states at runtime. State IDs are zero-based indices in `States`. Removing or moving states remaps parameter overrides and tracked state indices, but does not remap animation rule indices; update those rules separately. `AddParameter` adds a parameter without immediately applying the current state.

### 4. Configuring Animations

You can define transition rules between states in the `VisualState` component.

*   **Initial State**: The source state of the transition rule (distinct from the component's initial state). Use `*` to match any source.
*   **Target State**: The destination state. Use `*` to match any destination.
*   **Duration**: How long the transition takes (in seconds), evaluated through Vario when the transition starts.
*   **Easing**: The animation curve evaluated at normalized elapsed time; a null curve uses linear progress. The curve is resolved through Vario when the transition starts.

Rules with both states specified take priority over wildcard rules. Within each group, the first matching rule wins. In code, a wildcard state ID is `-1`. With no matching rule, or a duration of zero or less, the destination is applied immediately.

Animated transitions start from a snapshot of the actual target values. Interrupting a transition captures a new snapshot; requesting the same destination with animation does not restart it. `ChangeStateImmediately` also completes a running transition when its destination is already current. Destination values are evaluated as the transition progresses, and exact destination values are applied at completion.

`Timescale` selects `Time.deltaTime` (`Normal`) or `Time.unscaledDeltaTime` (`Unscaled`). `UpdateTime(float dt)` can advance an animation manually, but automatic `Update` also advances it while the component is enabled.

The default interpolation uses Vario's registered numeric operations with unclamped progress. Types without numeric operations switch from snapshot to destination at `0.5`. Transform parent and nested VisualState parameters expose `SwitchMoment` (default `0.5`). The parent parameter reparents with `worldPositionStays = false` and moves the object to the last sibling. The GameObject activity parameter keeps the object active while progress is below `0.99` if either endpoint is active.

## Creating Custom Parameters

To control a custom component or property, inherit from `StatioParameter<TComponent, TValue>`.

```csharp
using System;
using UnityEngine;
using Damdor.Statio;

// 1. Define the parameter class
namespace MyNamespace 
{
    [Serializable]
    [StatioParameter("Custom/MyParameter")]        
    public class MyCustomParameter : StatioParameter<CanvasGroup, float>
    {
        // 2. Implement how to get the value from the component
        protected override float GetValue(CanvasGroup target) => target.alpha;
    
        // 3. Implement how to set the value to the component
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    
        // 4. Optionally override the default Vario interpolation
        protected override float Lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
    }
}
```

Mark custom parameters with `[Serializable]` and `[StatioParameter("Group/Name")]`. The bundled Roslyn generator registers attributed concrete classes on editor load and runtime initialization. `StatioSettings.RegisterParameterType` can also register a type explicitly. Implement `GetValue` and `SetValue`; override `Lerp` only when the default interpolation does not fit the property.

Configure and attach a parameter from code using raw values or Vario expressions:

```csharp
// In a method with references to visualState and canvasGroup:
var visibleStateIndex = visualState.AddState("Visible");
var hiddenStateIndex = visualState.AddState("Hidden");

var parameter = new MyNamespace.MyCustomParameter
{
    Target = Damdor.Vario.VarioValue<CanvasGroup>.Raw(canvasGroup),
    DefaultValue = Damdor.Vario.VarioValue<float>.Raw(1f)
};
parameter.SetValue(hiddenStateIndex, Damdor.Vario.VarioValue<float>.Raw(0f));
visualState.AddParameter(parameter);
visualState.ChangeStateImmediately("Hidden");
```

`SetValue` stores the override; it does not apply it immediately.

Lifecycle operations such as snapshots, loading values, and saving component values are available through `IStatioParameterLifecycle`. Missing targets are skipped when loading or saving snapshots. Runtime parameter failures are reported individually so other parameters can still be processed. Errors are logged by default; use `StatioSettings.LogErrorsToConsole` and `RegisterErrorHandler` to customize reporting.

## Dependencies

*   `com.damdor.vario` version `1.0.0` (declared UPM dependency).
*   Unity UI (`UnityEngine.UI`) for UI parameters and `StatioButton`.
*   Optional: `com.coffee.ui-effect`. Its presence enables `DAMDOR_STATIO_UIEFFECT` through the assembly definition's version define.
