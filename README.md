# ACT-FAST: Accessible Care and Training for Stroke Rehabilitation

ACT-FAST is an academic Unity prototype exploring how interactive 3D environments can support accessible, engaging rehabilitation-inspired activities and stroke education.

The application features a desktop rehabilitation environment with four interactive stations where users can practise object placement, explore everyday activities, follow guided movement instructions, and access educational resources.

> **Note:** ACT-FAST is designed to run in desktop Play Mode or as a standalone desktop application. A VR headset is not required for the demonstration. This is an educational prototype, not a clinical rehabilitation product or a substitute for professional medical care.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Demonstration](#demonstration)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Controls](#controls)
- [Project Structure](#project-structure)
- [Main Scenes](#main-scenes)
- [Main Scripts](#main-scripts)
- [Team](#team)
- [Future Improvements](#future-improvements)
- [Disclaimer](#disclaimer)

## Project Overview

Rehabilitation and recovery can involve repetitive activities and exercises. ACT-FAST explores how an interactive digital environment can make rehabilitation-inspired activities more engaging while providing an accessible way to explore movement and stroke-related educational content.

The application takes place in a furnished 3D rehabilitation room containing four activity stations:

1. **Reach & Place:** Interact with and place objects in designated areas.
2. **Guided Movement:** Follow a structured, five-step guided arm movement activity.
3. **Everyday Activities:** Explore interactions with familiar everyday objects.
4. **Learning & Resources:** Browse general educational information.

Users can navigate the environment, interact with activities, track progress, and move between the main menu and rehabilitation room.

## Features

### 1. Main Menu

The main menu provides the application's starting point and navigation options.

- Start a rehabilitation session.
- Access educational resources.
- Exit the application.

### 2. Interactive Rehabilitation Environment

Users explore a furnished 3D rehabilitation room using first-person desktop controls.

The environment includes:

- Four dedicated activity stations.
- Interactive objects and placement areas.
- Station labels and interaction prompts.
- In-game interface elements for navigation and activity feedback.

### 3. Reach & Place Activity

An interactive object-placement activity designed around simple reaching and placement tasks.

- Locate and pick up interactive targets.
- Carry objects to designated placement areas.
- Receive feedback when attempting to place objects.
- Track progress toward the activity's completion goal.

### 4. Everyday Activities

An activity station featuring familiar objects, including a block, ball, and cup.

Users can explore and interact with these objects as part of the environment's everyday activity experience.

### 5. Guided Movement

A structured, five-step guided arm movement activity.

- Open the guided exercise instructions.
- Follow the instructions displayed on screen.
- Navigate between steps.
- Pause and resume the activity.
- Review previous steps and restart when needed.
- Receive completion feedback.

### 6. Learning & Resources

Educational content is available through the main menu and the rehabilitation room.

Users can browse informational resources without leaving the application.

### 7. Accessible Interface and Navigation

The application includes a consistent interface for menus, activity instructions, progress feedback, and navigation.

Keyboard and mouse controls allow users to explore the environment and interact with its activities without requiring VR equipment.

## Demonstration

The project is intended to be demonstrated through a desktop application walkthrough.

The demonstration can cover:

1. Navigating the main menu.
2. Accessing educational resources.
3. Starting a rehabilitation session.
4. Exploring the rehabilitation room.
5. Visiting the Reach & Place station and completing placement tasks.
6. Exploring the Everyday Activities station.
7. Following the Guided Movement activity.
8. Browsing resources within the rehabilitation room.
9. Using the pause and navigation controls.
10. Returning to the main menu.

## Technologies Used

| Technology | Purpose |
|---|---|
| Unity 6 | 3D environment and application development |
| C# | Application logic, activities, and interactions |
| Unity UI System | Menus, instructions, and interface elements |
| XR Interaction Toolkit | XR-related Unity project components and tooling |
| Git | Version control |
| GitHub | Source code management and collaboration |

## Getting Started

### Prerequisites

Before running the project, ensure you have:

- [Unity Hub](https://unity.com/)
- Unity 6, using a compatible editor version
- Git (for cloning the repository)
- Windows operating system (for the intended desktop demonstration)

### Clone the Repository

Clone the project using Git:

```bash
git clone https://github.com/fayy-lee/CSI4104_Group7.git
```

Navigate to the project directory:

```bash
cd CSI4104_Group7
```

### Open the Project in Unity

1. Open Unity Hub.
2. Select **Add Project**.
3. Navigate to the cloned repository.
4. Select the project folder.
5. Open it using a compatible Unity Editor version.
6. Navigate to `Assets/Scenes`.
7. Open the `MainMenu` scene.
8. Press **Play** to launch the application.

### Running a Standalone Build

To run ACT-FAST without opening the Unity Editor:

1. Open the project in Unity.
2. Navigate to **File → Build Profiles**.
3. Select the Windows build platform.
4. Ensure both scenes are included and enabled:
   - `MainMenu`
   - `RehabilitationRoom`
5. Select **Build**.
6. Choose an output directory.
7. Open the generated executable to launch ACT-FAST.

## Controls

| Input | Action |
|---|---|
| W | Move forward |
| A | Move left |
| S | Move backward |
| D | Move right |
| Mouse | Look around when the pointer is locked |
| Tab | Toggle pointer lock for menu interaction |
| E | Interact with objects and activity stations |
| Space / Right Arrow | Advance through guided movement |
| Left Arrow | Return to the previous guided movement step |
| P | Pause or resume guided movement |
| Esc | Open pause controls or close an activity panel |

## Project Structure

The Unity project is organized into the following directories:

```text
CSI4104_Group7/
│
├── Assets/
│   ├── Audio/
│   ├── Materials/
│   ├── Models/
│   ├── Prefabs/
│   ├── Samples/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   └── RehabilitationRoom.unity
│   ├── Scripts/
│   └── UI/
│
├── Packages/
├── ProjectSettings/
├── .gitignore
└── README.md
```

## Main Scenes

| Scene | Description |
|---|---|
| `MainMenu` | Application landing page, navigation, and access to educational resources |
| `RehabilitationRoom` | Main interactive environment containing the four activity stations |

## Main Scripts

The application uses C# scripts to manage navigation, player movement, interactions, activities, and interface elements.

| Script | Responsibility |
|---|---|
| `MainMenuManager.cs` | Handles main menu navigation and educational resources |
| `PlayerController.cs` | Manages first-person movement and camera controls |
| `PlayerInteraction.cs` | Handles interactions with objects, targets, and placement areas |
| `RoomUIManager.cs` | Manages in-game interface elements and navigation |
| `ArmExerciseManager.cs` | Controls the guided arm movement activity |
| `ResourceStationManager.cs` | Manages educational resources within the rehabilitation room |
| `WelcomeManager.cs` | Handles the welcome message when entering the environment |
| `AccessibleUIFactory.cs` | Provides shared interface creation and styling components |

## Team

Developed by:

- Lacia Cummings
- Sanika Sisodia
- Fay Lee

**University of Ottawa**  
CSI4104 - Software Engineering Project

## Future Improvements

Potential future enhancements include:

- Additional rehabilitation-inspired activities and interactive exercises.
- Expanded educational resources.
- More detailed activity progress tracking.
- Further accessibility and interface customization options.
- Enhanced environmental interactions and feedback.
- Optional VR headset and motion controller support.

## Disclaimer

ACT-FAST is an academic software prototype developed to explore interactive rehabilitation concepts and stroke education. It is intended for educational and demonstration purposes only. It is not a clinical rehabilitation product, does not provide personalized medical guidance, and is not a substitute for professional medical advice, assessment, or rehabilitation treatment.
