# Statio

A powerful state management system for Unity that allows you to define, transition, and animate between different visual configurations of your GameObjects.

## Features

*   **State-Based Configuration**: Define multiple visual states (e.g., "Normal", "Hover", "Pressed", "Disabled") for a single GameObject.
*   **Parameter Control**: Control various component properties (Color, Position, Scale, Alpha, etc.) per state.
*   **Animated Transitions**: Smoothly interpolate between states with configurable duration and easing curves.
*   **Variable Storage Integration**: Leverage the `VariableStorage` system to drive state values and transition parameters dynamically.
*   **Extensible**: Create custom parameters to control any property of any component.

## Installation

This package is available via UPM.

## Basic Usage

### 1. Setting up a Visual State Component

Add the `VisualState` component to a GameObject.

1.  **Define States**: In the Inspector, add state names to the "States" list (e.g., "Idle", "Active").
2.  **Add Parameters**: Add parameters to control specific properties. For example, a `ColorParameter` to change a SpriteRenderer's color.
3.  **Configure Values**:
    *   Set a **Default Value** for the parameter.
    *   Add overrides for specific states where the value should differ from the default.

### 2. Controlling States from Code

You can change states using the `VisualState` API.

```csharp
using Damdor.Statio;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private VisualState visualState;

    public void OnPointerEnter()
    {
        // Smoothly transition to "Hover" state
        visualState.ChangeState("Hover");
    }

    public void OnPointerExit()
    {
        // Smoothly transition back to "Normal" state
        visualState.ChangeState("Normal");
    }

    public void OnClick()
    {
        // Instantly snap to "Pressed" state without animation
        visualState.ChangeStateImmediately("Pressed");
    }
}
```

### 3. Configuring Animations

You can define transition rules between states in the `VisualState` component.

*   **Initial State**: The starting state of the transition.
*   **Target State**: The destination state.
*   **Duration**: How long the transition takes (in seconds).
*   **Easing**: The animation curve to use for interpolation.

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
    public class MyCustomParameter : StatioParameter<CanvasGroup, float>
    {
        // 2. Implement how to get the value from the component
        protected override float GetValue(CanvasGroup target) => target.alpha;
    
        // 3. Implement how to set the value to the component
        protected override void SetValue(CanvasGroup target, float value) => target.alpha = value;
    
        // 4. Implement interpolation logic
        protected override float Lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
    }
}
```

then create file `statio_settings.json` in `Resources` file and register this parameter:
```json
{
  "parameters": {
    "myCustomName": "MyNamespace.MyCustomParameter"
  }
}
```

## Dependencies

*   `com.damdor.foundations`
*   `com.damdor.variablestorage`
