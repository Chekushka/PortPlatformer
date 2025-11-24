Practice Platformer Project

A third-person platformer inspired by Super Mario Odyssey, built in Unity. This project features a robust input management system, dynamic UI, and controller support.

Key Features
3D Movement: Camera-relative character movement using a CharacterController.

Camera System: Modern camera control using Cinemachine.

Advanced Input System:

Built on the new Unity Input System.

Dynamic Icon Switching: UI prompts automatically switch between Keyboard, Xbox, PlayStation, and Switch icons based on the active device.

Key Rebinding: Fully functional menu to rebind keys at runtime with visual feedback.

Hybrid Fallback: Displays high-quality icons for known keys and text fallback (e.g., "F12") for others.

Polished UI:

Tabbed Menus: Settings menu with animated tab switching (using DOTween).

Gamepad Navigation: Full support for UI navigation with gamepads, including auto-scrolling lists and "remembered" selections.

Animations: Interactive button scaling and device change notification banners.

Pause System: Robust IPausable interface and PauseManager singleton to handle game states and UI overlays.

