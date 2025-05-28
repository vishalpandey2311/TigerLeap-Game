# TigerLeap Card Memory Game

A challenging 3D memory card matching game built with Unity where players must find pairs of cards matching their hand cards within time limits.

## Table of Contents
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

## Game Overview

TigerLeap is a memory-based card matching game where players are given a hand of 3 cards and must find matching pairs from a 5x9 grid of face-down cards. The game combines memory skills with time pressure across three difficulty levels.

### Core Mechanics
- **Memory Phase**: 10-second initial viewing period to memorize card positions
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
- Smooth card movement to collection grid
- Visual completion indicators with checkmarks
- Professional UI with multiple panels
- Responsive button interactions

## How to Play

### Getting Started
1. **Launch Game**: Start the application to see the difficulty selection screen
2. **Choose Difficulty**: Select Easy (1 min), Intermediate (3 min), or Hard (5 min)
3. **Read Instructions**: Review the "How to Play" panel if needed

### Gameplay Flow
1. **Memorization Phase** (10 seconds):
   - All cards are face-up showing their designs
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
| **Easy** | 1 minute | Beginner-friendly with generous time |
| **Intermediate** | 3 minutes | Moderate challenge for regular players |
| **Hard** | 5 minutes | Maximum challenge for expert players |

## Controls

### Mouse Controls
- **Left Click**: Flip cards, interact with UI buttons
- **Pause Button**: Top-center of screen during gameplay

### UI Navigation
- **Pause Menu**: Resume, Info, or Quit options
- **Win/Loss Screens**: Play Again or Exit options
- **Info Button**: Access instructions during pause

## Technical Implementation

### Architecture
The game follows a modular architecture with clear separation of concerns:

- **GameManager**: Central game state management and coordination
- **CardSpawner**: Handles card generation and grid setup
- **CardController**: Individual card behavior and interactions
- **AudioManager**: Centralized audio system with singleton pattern

### Key Systems

#### Card System
- **Grid Generation**: 5x9 grid with configurable spacing
- **Hand Selection**: Random unique cards for player hand
- **Deck Preparation**: Ensures 4 copies of each hand card in grid
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
  - Deck preparation ensuring hand card matches
  - Collection grid positioning system
  - Player hand display

#### `CardController.cs`
- **Purpose**: Individual card behavior and interactions
- **Key Features**:
  - Smooth flip animations using coroutines
  - Click detection and interaction handling
  - Match validation against player hand
  - Automatic face-down after viewing period
  - Movement animation to collection grid

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

### Supporting Scripts

#### UI Controllers
- Button interaction handlers
- Panel visibility management
- Text display updates
- Progress indicators

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
- **Dynamic Loading**: Sounds loaded and configured at runtime
- **Volume Control**: Global master volume adjustment
- **Mute Functionality**: Complete audio disable option
- **Pitch Variation**: Configurable for variety

## Installation

### For Players
1. Download the built game executable
2. Extract to desired location
3. Run the executable file
4. Ensure audio drivers are installed for sound

### For Developers
1. **Unity Version**: 2022.3 LTS or later recommended
2. **Clone Repository**: 
   ```bash
   git clone [repository-url]
   ```
3. **Open in Unity**: File → Open Project → Select folder
4. **Import Assets**: Ensure all prefabs and audio files are imported
5. **Build Settings**: Configure for target platform

### Required Assets
- **Card Prefabs**: Individual card models with textures
- **Audio Clips**: All sound effects and music files
- **UI Sprites**: Button textures and backgrounds
- **Materials**: Card face and back materials

## System Requirements

### Minimum Requirements
- **OS**: Windows 10, macOS 10.14, or Ubuntu 18.04
- **RAM**: 4 GB
- **Storage**: 500 MB available space
- **Graphics**: DirectX 11 compatible
- **Audio**: Integrated sound card

### Recommended Requirements
- **OS**: Windows 11, macOS 12+, or Ubuntu 20.04+
- **RAM**: 8 GB or more
- **Storage**: 1 GB available space
- **Graphics**: Dedicated graphics card
- **Audio**: Dedicated sound card for enhanced audio

### Development Requirements
- **Unity**: 2022.3 LTS or later
- **IDE**: Visual Studio 2022 or Visual Studio Code
- **Platform SDKs**: Based on target deployment platforms

## Troubleshooting

### Common Issues

#### Audio Not Playing
- Check system volume and mute settings
- Verify audio drivers are installed
- Test with other applications

#### Cards Not Responding
- Ensure game is not paused
- Wait for initial countdown to complete
- Check mouse/input device functionality

#### Performance Issues
- Close other applications
- Check system meets minimum requirements
- Reduce screen resolution if needed

#### Game Won't Start
- Verify all files extracted properly
- Check antivirus isn't blocking execution
- Run as administrator if needed

## Development Notes

### Future Enhancements
- **Multiplayer Support**: Add competitive or cooperative modes
- **Card Themes**: Multiple visual themes and card sets
- **Achievement System**: Unlock rewards for performance milestones
- **Leaderboards**: Online score tracking and competition
- **Mobile Support**: Touch controls for mobile devices

### Known Limitations
- Fixed grid size (5x9)
- Limited to 3 hand cards
- Single-player only
- Desktop platforms only

## License

This project is developed as an educational assignment. All rights reserved.

## Contact

For questions, issues, or suggestions regarding this game, please contact the development team through the appropriate channels.

---

*TigerLeap Card Memory Game - Challenge your memory, race against time!*
