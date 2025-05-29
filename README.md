# TigerLeap Card Memory Game

A challenging 3D memory card matching game built with Unity, where players must find pairs of cards matching their hand cards within time limits.

## Demo & Screenshots

![Game Screenshot](screenshots/gameplay.png)
*Main gameplay showing the 5x9 card grid and player hand*

![Difficulty Selection](screenshots/Difficulty.png)
*Difficulty selection screen*

### 🎮 [Live Demo](https://your-demo-link.com) | 📦 [Download Latest Release](https://github.com/vishalpandey2311/TigerLeap-Game/releases)

## Table of Contents
- [Demo & Screenshots](#demo--screenshots)
- [Game Overview](#game-overview)
- [Features](#features)
- [How to Play](#how-to-play)
- [Difficulty Levels](#difficulty-levels)
- [Controls](#controls)
- [Technical Implementation](#technical-implementation)
- [Scripts Overview](#scripts-overview)
- [Audio System](#audio-system)
- [Installation](#installation)
- [System Requirements](#system-requirements)
- [Development Setup](#development-setup)
- [Contributing](#contributing)
- [Troubleshooting](#troubleshooting)
- [Changelog](#changelog)
- [License](#license)

## Game Overview

TigerLeap is a memory-based card matching game where players are given a hand of 3 cards and must find matching pairs from a 5x9 grid of face-down cards. The game combines memory skills with time pressure to create an engaging and challenging experience.

### Core Mechanics
- **Memory Phase**: 10-second initial viewing period to memorise card positions
- **Matching Phase**: Find pairs matching your hand cards before time runs out
- **Collection System**: Successfully matched cards move to a collection grid
- **Scoring**: Track attempts and completion time

## Features

### Game Features
- 🎯 Three difficulty levels (Easy, Intermediate, Hard)
- ⏱️ Time-based challenges with countdown timers
- 🃏 Dynamic card generation with unique hand cards
- 🎵 Immersive audio system with sound effects
- ⏸️ Pause/Resume functionality
- 📊 Performance tracking (attempts and time)
- 🏆 Win/Loss detection with appropriate feedback

### Visual Features
- 3D card flip animations
- Smooth card movement to the collection grid
- Visual completion indicators with checkmarks
- Professional UI with multiple panels
- Responsive button interactions

## How to Play

### Getting Started
1. **Launch Game**: Start the application to see the difficulty selection screen
2. **Choose Difficulty**: Select Easy (5 min), Intermediate (3 min), or Hard (1 min)
3. **Read Instructions**: Review the "How to Play" panel if needed

### Gameplay Flow
1. **Memorization Phase** (10 seconds):
   - All cards are face-up, showing their designs
   - Study the positions of cards matching your hand (bottom of screen)
   - Use this time to plan your strategy

2. **Game Phase**:
   - Cards flip face-down automatically after 10 seconds
   - Click cards to flip them and find matches
   - Successfully matched cards move to the collection grid
   - Continue until all hand cards are matched or time runs out

3. **Completion**:
   - **Win**: Match all cards before time expires
   - **Lose**: Time runs out before completing all matches

### Hand Cards
- Located at the bottom of the screen
- Shows 3 unique cards you need to find pairs for
- Each card type has exactly 4 copies in the grid
- Cards remain visible throughout the game for reference

## Difficulty Levels

| Difficulty | Time Limit | Challenge Level |
|------------|------------|-----------------|
| **Easy** | 5 minutes | Beginner-friendly with generous time |
| **Intermediate** | 3 minutes | Moderate challenge for regular players |
| **Hard** | 1 minute | Maximum challenge for expert players |

## Controls

### Mouse Controls
- **Left Click**: Flip cards, interact with UI buttons
- **Pause Button**: Top-center of screen during gameplay

### UI Navigation
- **Pause Menu**: Resume, Info, or Quit options
- **Win/Loss Screens**: Play Again or Exit options
- **Info Button**: Access instructions during pause

## Technical Implementation

### Technology Stack
- **Engine**: Unity 2022.3 LTS
- **Language**: C# (.NET Framework)
- **Graphics**: Universal Render Pipeline (URP)
- **Audio**: Unity Audio System
- **Platform**: Windows, macOS, Linux

### Architecture
The game follows a modular architecture with clear separation of concerns:

- **GameManager**: Central game state management and coordination
- **CardSpawner**: Handles card generation and grid setup
- **CardController**: Individual card behaviour and interactions
- **AudioManager**: Centralised audio system with singleton pattern

### Key Systems

#### Card System
- **Grid Generation**: 5x9 grid with configurable spacing
- **Hand Selection**: Random unique cards for player's hand
- **Deck Preparation**: Ensures 4 copies of each hand card in the grid
- **Collection Grid**: 4x3 organized display for matched cards

#### Audio System
- **Singleton Pattern**: Global audio management
- **Sound Categories**: Match sounds, UI feedback, background music
- **Volume Control**: Master volume with mute functionality

#### UI System
- **Panel Management**: Multiple UI states (Menu, Game, Pause, Win/Loss)
- **Responsive Design**: Scales across different screen sizes
- **Visual Feedback**: Completion indicators and progress tracking

## Scripts Overview

### Core Scripts

#### `GameManager.cs`
- **Purpose**: Central game controller and state management
- **Key Features**:
  - Game state transitions (Start, Pause, Win, Lose)
  - Timer management and countdown display
  - Score tracking (attempts, time)
  - Audio coordination
  - UI panel management

#### `CardSpawner.cs`
- **Purpose**: Card generation and grid setup
- **Key Features**:
  - 5x9 grid layout with configurable spacing
  - Random hand card selection (3 unique cards)
  - Deck preparation, ensuring hand card matches
  - Collection grid positioning system
  - Player hand display

#### `CardController.cs`
- **Purpose**: Individual card behaviour and interactions
- **Key Features**:
  - Smooth flip animations using coroutines
  - Click detection and interaction handling
  - Match validation against the player's hand
  - Automatic face-down after viewing period
  - Movement animation to the collection grid

#### `PlayerHandCardController.cs`
- **Purpose**: Special controller for player hand cards
- **Key Features**:
  - Static display (no flipping)
  - Reference for matching validation
  - Visual distinction from grid cards

#### `AudioManager.cs`
- **Purpose**: Centralized audio system
- **Key Features**:
  - Singleton pattern for global access
  - Dynamic audio source creation
  - Volume and mute controls
  - Sound effect categories

### Data Flow
1. **Initialization**: GameManager → CardSpawner → Individual Cards
2. **User Input**: CardController → GameManager → UI Updates
3. **Audio**: GameManager → AudioManager → Sound Playback
4. **Game State**: GameManager ↔ All Systems

## Audio System

### Sound Effects
- **Correct Match**: Satisfying success sound
- **Wrong Match**: Gentle failure feedback
- **Game Win**: Celebratory victory music
- **Column Complete**: Progress milestone sound
- **Game Loss**: Appropriate failure sound
- **Countdown**: Tension-building background music

### Audio Features
- **Dynamic Loading**: Sounds are loaded and configured at runtime
- **Volume Control**: Global master volume adjustment
- **Mute Functionality**: Complete audio disable option
- **Pitch Variation**: Configurable for variety

## Installation

### Quick Start (Players)
1. **Download**: Get the latest release from the [Releases page](https://github.com/vishalpandey2311/TigerLeap-Game/releases)
2. **Extract**: Unzip the downloaded file to your desired location
3. **Run**: Execute `TigerLeap-Game.exe` (Windows) or equivalent for your platform
4. **Play**: Select difficulty and start playing!

### System Requirements

#### Minimum Requirements
- **OS**: Windows 10, macOS 10.14, or Ubuntu 18.04
- **RAM**: 4 GB
- **Storage**: 500 MB available space
- **Graphics**: DirectX 11 compatible
- **Audio**: Integrated sound card

#### Recommended Requirements
- **OS**: Windows 11, macOS 12+, or Ubuntu 20.04+
- **RAM**: 8 GB or more
- **Storage**: 1 GB available space
- **Graphics**: Dedicated graphics card
- **Audio**: Dedicated sound card for enhanced audio

## Development Setup

### Prerequisites
- **Unity**: 2022.3 LTS or later
- **IDE**: Visual Studio 2022 or Visual Studio Code with C# extension
- **Git**: For version control

### Getting Started
1. **Clone the repository**:
   ```bash
   git clone https://github.com/vishalpandey2311/TigerLeap-Game.git
   cd TigerLeap-Game
   ```

2. **Open in Unity**:
   - Launch Unity Hub
   - Click "Add" and select the cloned folder
   - Open the project with Unity 2022.3 LTS

3. **Build and Run**:
   - File → Build Settings
   - Select your target platform
   - Click "Build and Run"

### Project Structure
```
Assets/
├── Scripts/           # C# game logic
├── Prefabs/          # Card and UI prefabs
├── Materials/        # Card textures and materials
├── Audio/           # Sound effects and music
├── Scenes/          # Game scenes
└── UI/              # User interface assets
```

### Development Workflow
1. **Feature Development**: Create feature branches
2. **Testing**: Test on multiple platforms
3. **Code Review**: Review pull requests
4. **Integration**: Merge to main branch
5. **Release**: Create releases with proper versioning

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### How to Contribute
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature')
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding Standards
- Follow C# naming conventions
- Comment on complex algorithms
- Write unit tests for new features
- Ensure cross-platform compatibility

## Troubleshooting

### Common Issues

#### Audio Not Playing
- Check system volume and mute settings
- Verify audio drivers are installed
- Test with other applications

#### Cards Not Responding
- Ensure the game is not paused
- Wait for the initial countdown to complete
- Check mouse/input device functionality

#### Performance Issues
- Close other applications
- Check system meets the minimum requirements
- Reduce screen resolution if needed

#### Game Won't Start
- Verify all files were extracted properly
- Check that the antivirus isn't blocking execution
- Run as an administrator if needed

### Getting Help
- 📝 [Open an Issue](https://github.com/vishalpandey2311/TigerLeap-Game/issues)
- 💬 [Start a Discussion](https://github.com/vishalpandey2311/TigerLeap-Game/discussions)
- 📧 Contact: [your-email@example.com]

## Changelog

### v1.0.0 (Latest)
- Initial release
- Core gameplay mechanics
- Three difficulty levels
- Audio system implementation
- Cross-platform support

See [CHANGELOG.md](CHANGELOG.md) for detailed version history.

## Roadmap

### Planned Features
- **v1.1.0**: 
  - Leaderboards and score tracking
  - Additional card themes
  - Settings menu with customizable options

- **v1.2.0**:
  - Multiplayer support
  - Achievement system
  - Mobile platform support

- **v2.0.0**:
  - Custom level editor
  - Procedural card generation
  - Steam integration

## Performance & Analytics

- **Build Size**: ~150MB (depending on platform)
- **Memory Usage**: ~200MB during gameplay
- **Startup Time**: < 3 seconds on recommended hardware

## License

This project is developed as an educational assignment. All rights reserved.

### Third-Party Assets
- Unity Engine: [Unity License](https://unity3d.com/legal/licenses/Unity_Personal_General_License)
- Audio files: Custom-created or royalty-free
- Card graphics: Original artwork

## Acknowledgments

- Unity Technologies for the game engine
- The Unity community for tutorials and support
- Beta testers for valuable feedback

## Contact

**Developer**: Vishal Pandey  
**GitHub**: [@vishalpandey2311](https://github.com/vishalpandey2311)  
**Project Link**: [https://github.com/vishalpandey2311/TigerLeap-Game](https://github.com/vishalpandey2311/TigerLeap-Game)

---

*TigerLeap Card Memory Game - Challenge your memory, race against time!*

⭐ **Found this project helpful? Give it a star!** ⭐
