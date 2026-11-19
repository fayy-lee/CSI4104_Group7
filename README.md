# ACT-FAST: Accessible Care and Training for Stroke Rehabilitation

ACT-FAST is an academic Unity prototype exploring an accessible desktop
3D environment with optional guided activities, progress feedback, and
general educational resources.

Users can explore four activity stations, move and place familiar
objects, follow self-paced movement instructions, and browse learning
content.

> **Note:** The demonstration runs in desktop Play Mode and does not
> require a VR headset. It is an educational prototype, not a clinical
> rehabilitation product or personalized medical treatment.

## Table of Contents

-   [Project Overview](#project-overview)
-   [Features](#features)
-   [Demonstration](#demonstration)
-   [Technologies Used](#technologies-used)
-   [Getting Started](#getting-started)
-   [Controls](#controls)
-   [Project Structure](#project-structure)
-   [Main Scenes](#main-scenes)
-   [Main Scripts](#main-scripts)
-   [Team](#team)
-   [Future Improvements](#future-improvements)
-   [Disclaimer](#disclaimer)

## Project Overview

Rehabilitation can involve repetitive exercises and activities that
users may find difficult to engage with consistently. RehabVR aims to
demonstrate how interactive digital environments can make rehabilitation
activities more engaging and accessible.

The project features a virtual rehabilitation room where users can
explore their surroundings and interact with different stations.

The prototype includes:

-   A main menu for navigating the application.
-   A first-person 3D rehabilitation environment.
-   An interactive object collection activity.
-   A guided arm exercise station.
-   Educational resources accessible throughout the experience.
-   A progress tracking system for interactive activities.

## Features

### 1. Main Menu

The main menu provides an entry point to the application and allows
users to:

-   Start a rehabilitation session.
-   Access educational resources.
-   Exit the application.

### 2. Interactive Rehabilitation Environment

Users can explore a 3D rehabilitation room through first-person
controls.

The environment includes:

-   A furnished rehabilitation room.
-   Interactive activity stations.
-   Educational resource stations.
-   Navigation and interaction prompts.

### 3. Object Collection Activity

Users can participate in a simple object collection activity within the
rehabilitation environment.

-   Locate interactive objects.
-   Interact with objects using the keyboard.
-   Collect objects to increase the activity score.
-   Track progress toward the completion goal.

### 4. Guided Arm Exercise

The arm exercise station provides a structured, five-step guided
activity.

-   Approach the exercise station.
-   Open the exercise instructions.
-   Follow the instructions displayed on screen.
-   Advance through the activity using the keyboard.
-   Exit the activity when finished.

### 5. Educational Resources

Educational resources are available from both the main menu and the
rehabilitation environment.

Users can access informational content without leaving the application.

## Demonstration

The project is demonstrated through a screen-recorded walkthrough of the
desktop application.

The demonstration covers:

1.  Navigating the main menu.
2.  Accessing educational resources.
3.  Entering the rehabilitation environment.
4.  Exploring the room using first-person controls.
5.  Completing the object collection activity.
6.  Interacting with the guided arm exercise station.
7.  Accessing educational resources within the room.
8.  Returning to the main menu.

## Technologies Used

| Technology | Purpose |
|---|---|
| Unity | 3D environment and application development |
| C# | Application logic and interaction scripts |
| Unity UI System | Menus, instructions, and interface elements |
| Git | Version control |
| GitHub | Source code management and collaboration |

## Getting Started

### Prerequisites

Before running the project, ensure you have:

-   [Unity Hub](https://unity.com/)
-   A compatible Unity Editor version
-   Windows operating system (for the standalone build)

### Clone the Repository

Clone the project using Git:

``` bash
git clone https://github.com/fayy-lee/CSI4104_Group7.git
```

Navigate to the project directory:

``` bash
cd CSI4104_Group7
```

### Open the Project in Unity

1.  Open Unity Hub.
2.  Select **Add Project**.
3.  Navigate to the cloned repository.
4.  Select the project folder.
5.  Open the project using a compatible Unity Editor version.
6.  Navigate to `Assets/Scenes`.
7.  Open the `MainMenu` scene.
8.  Press **Play** to launch the application.

### Running a Standalone Build

To run the application without opening the Unity Editor:

1.  Open the project in Unity.
2.  Navigate to **File → Build Profiles**.
3.  Select the Windows build platform.
4.  Ensure the following scenes are included in the build:
    -   `MainMenu`
    -   `RehabilitationRoom`
5.  Select **Build**.
6.  Choose an output directory.
7.  Open the generated executable to launch ACT-FAST.

## Controls

| Input | Action |
|---|---|
| W | Move forward |
| A | Move left |
| S | Move backward |
| D | Move right |
| Mouse (locked) | Look around |
| Tab | Toggle pointer lock for menu interaction |
| E | Interact with objects and activity stations |
| Space / Right Arrow | Advance through guided movement |
| Left Arrow | Previous guided movement step |
| P | Pause or resume guided movement |
| Esc | Open pause controls or close an activity panel |

## Project Structure

The Unity project is organized into the following directories:

``` text
CSI4104_Group7/
│
├── Assets/
│   ├── Audio/
│   ├── Materials/
│   ├── Models/
│   ├── Prefabs/
│   ├── Scenes/
│   │   ├── MainMenu
│   │   └── RehabilitationRoom
│   ├── Scripts/
│   └── UI/
│
├── Packages/
├── ProjectSettings/
├── .gitignore
└── README.md
```

## Main Scenes

  Scene                  Description
  ---------------------- ---------------------------------------------
  `MainMenu`             Application landing page and navigation
  `RehabilitationRoom`   Main interactive rehabilitation environment

## Main Scripts

The application uses C# scripts to manage navigation, player movement,
interactions, and activities.

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `MainMenuManager.cs`                Handles main menu navigation and
                                      educational resources

  `PlayerController.cs`               Manages first-person player
                                      movement and camera controls

  `PlayerInteraction.cs`              Handles interactions with objects
                                      and activity stations

  `RoomUIManager.cs`                  Manages in-game UI and navigation
                                      back to the main menu

  `ArmExerciseManager.cs`             Controls the guided arm exercise
                                      activity

  `ResourceStationManager.cs`         Manages educational resources
                                      within the rehabilitation room

  `WelcomeManager.cs`                 Handles the welcome message when
                                      entering the environment
  -----------------------------------------------------------------------

## Team

Developed by:

-   Lacia Cummings
-   Sanika Sisodia
-   Fay Lee

**University of Ottawa**
CSI4104 - VR Project

## Future Improvements

Potential future enhancements include:

-   Integration with VR headsets and motion controllers.
-   Additional rehabilitation activities and interactive exercises.
-   More extensive progress tracking.
-   Customizable accessibility settings.
-   Expanded educational resources.
-   Improved environmental interaction and feedback.

## Disclaimer

RehabVR is an academic software prototype developed to demonstrate
interactive rehabilitation concepts. It is intended for educational and
demonstration purposes only and is not a substitute for professional
medical advice, assessment, or rehabilitation treatment.
