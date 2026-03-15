# Player State Machine Architecture

## Overview
The player movement system has been refactored to use a **State Machine Pattern**, where each movement state has its own logic encapsulated in separate classes.

## State Diagram

```
                      ┌─────────────────────────────────────┐
                      │        IDLE STATE                   │
                      │  - Standing still on ground         │
                      │  - No movement input                │
                      └─────────────────────────────────────┘
                               ▲  │  ▼
                 Jump Input ───┘  │  └─── Movement Input
                                  │
                            Sit Input ▼
                      ┌─────────────────────────────────────┐
                      │      SITTING STATE                  │
                      │  - Player is seated                 │
                      │  - Upperbody layer enabled          │
                      └─────────────────────────────────────┘
                               │
                Movement Input  │
                       ▼─────┬─┘
                             │
                    ┌────────┴──────────┐
                    │                   │
                    ▼                   ▼
            ┌──────────────┐    ┌──────────────────┐
            │ MOVING STATE │    │ SPRINTING STATE  │
            │ - Walking    │    │ - Running fast   │
            │ - Normal speed──Sprint Button──High speed
            └──────────────┘    └──────────────────┘
                    │                   │
                    └────────┬──────────┘
                             │
                      Jump Input ▼
                      ┌─────────────────────────────────────┐
                      │      JUMPING STATE                  │
                      │  - In the air                       │
                      │  - Can double jump                  │
                      │  - Returns to Move/Idle on ground   │
                      └─────────────────────────────────────┘
```

## State Classes

### PlayerState (Base Class)
- **Purpose**: Abstract base class defining the interface for all states
- **Methods**:
  - `OnEnter()`: Called when entering the state
  - `Update()`: Called every frame while in the state
  - `OnExit()`: Called when exiting the state

### IdleState
- **Transitions**:
  - → Moving: When movement input is provided
  - → Sprinting: When sprint + movement input
  - → Jumping: When jump input
  - → Sitting: When sit input
- **Behavior**: 
  - Applies gravity
  - Waits for player input to transition

### MovingState
- **Transitions**:
  - → Idle: When movement input stops
  - → Sprinting: When sprint button pressed
  - → Jumping: When jump input
  - → Sitting: When sit input
  - Auto-exit sitting: On any movement
- **Behavior**:
  - Moves character at walk speed
  - Handles acceleration/deceleration
  - Smooth speed transition

### SprintingState
- **Transitions**:
  - → Moving: When sprint released but still moving
  - → Idle: When all input stops
  - → Jumping: When jump input
  - → Sitting: When sit input
  - Auto-exit sitting: On any movement
- **Behavior**:
  - Moves character at sprint speed
  - Handles acceleration/deceleration
  - Higher speed than moving state

### JumpingState
- **Transitions**:
  - → Idle: When grounded with no movement
  - → Moving: When grounded with movement input
  - → Sprinting: When grounded with sprint + movement
  - Double Jump: Can jump again mid-air (if CanJump)
- **Behavior**:
  - Applies initial jump impulse
  - Allows air movement
  - Applies gravity during flight
  - Returns to appropriate state on landing

### SittingState
- **Transitions**:
  - → Idle: When sit input (toggle)
  - → Moving: When movement input
  - → Sprinting: When sprint + movement
  - → Jumping: When jump input
  - Auto-exit: On movement input
- **Behavior**:
  - Enables UpperBody animation layer
  - Disables movement
  - Can still jump up
  - Auto-stand when moving

## Key Features

### 1. **Separation of Concerns**
Each state handles only its own logic, making the code:
- Easier to read
- Easier to debug
- Easier to extend with new states

### 2. **Clear Transitions**
State transitions are explicit and documented in each state's Update() method.

### 3. **Encapsulation**
Each state maintains its own data (like currentSpeed) without affecting other states.

### 4. **Reusability**
States can be created and destroyed, allowing for proper cleanup and state-specific initialization.

### 5. **Animation Integration**
Each state manages its own animator parameters:
- `Speed`: Movement speed
- `YVelocity`: Vertical velocity for falling
- `XInput`: Horizontal input direction
- `IsSitting`: Sitting state toggle
- `IsGrounded`: Ground collision detection

## Adding New States

To add a new state (e.g., DashState):

1. Create a new file: `DashState.cs`
2. Inherit from `PlayerState`
3. Implement `OnEnter()`, `Update()`, and `OnExit()`
4. Define transitions in `Update()`
5. Add the transition logic in relevant states

```csharp
public class DashState : PlayerState
{
    private float dashDuration = 0.5f;
    private float elapsedTime = 0f;

    public DashState(PlayerController controller, Animator animator) : base(controller, animator)
    {
    }

    public override void OnEnter()
    {
        elapsedTime = 0f;
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= dashDuration)
        {
            controller.SetState(new IdleState(controller, animator));
            return;
        }

        // Dash logic here
    }
}
```

## Input Handling

The `PlayerInputHandler` provides these input properties:
- `MoveInput`: Vector2 with WASD/Stick input
- `IsSprinting`: Boolean from shift/RT button
- `JumpPressed`: Boolean from space/A button
- `SitPressed`: Boolean from C/LB button

Methods:
- `ResetJumpInput()`: Called after jump is processed
- `ResetSitInput()`: Called after sit is processed

## Performance Considerations

- **Memory**: Each state is instantiated when needed (except base states)
- **CPU**: Only active state Update() is called per frame
- **Garbage Collection**: Consider object pooling for frequently created states

## Debugging Tips

To debug state transitions, add this to PlayerController:

```csharp
private void OnGUI()
{
    GUI.Label(new Rect(10, 10, 200, 50), $"Current State: {m_CurrentState.GetType().Name}");
}
```

