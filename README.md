# Tetris Clone - Xamarin Project

## Overview

Welcome to my first Xamarin project, created 2022, published today - a Tetris clone built for educational and personal purposes. The objective of this project was to create a functional Tetris clone in the shortest time possible while familiarizing myself with the Xamarin IDE and its capabilities. This project is designed for mobile devices and features adaptable UI elements that scale appropriately across different screen sizes.

> **Note:** This project is purely for personal use and will not be monetized, sold, or distributed in any form or way. It is not intended for commercial use or distribution and it is a personal project to demonstrate personal skills and growth.

## Features

- **Home Page:** 
  - Start the game or access additional pages such as Daily Challenges or Profile.
  - View daily challenges that reward experience points (XP) upon completion.

- **Game Page:**
  - Classic Tetris gameplay with two modes:
    - **Classic Mode:** Play without a time limit and aim for the highest score.
    - **Time-Limited Mode:** Score as many points as possible within 3 minutes.
  - Displays current score, high score, and total lines cleared.
  - Pause menu with options to continue, start a new game, or quit.

- **Settings Page:**
  - Adjust settings such as:
    - Field Size
    - Touchscreen Sensitivity
    - Sounds and Music
    - Vibrations
    - Ghost Piece
  - Restore default settings.
  - Access the "How to Play" page with game rules.

- **Profile Page:**
  - View player information including:
    - Username
    - Current level and XP
    - Unlocked avatars (based on current level)
    - Total lines cleared
    - High scores

- **Challenges Page:**
  - View a list of daily challenges and track XP and level progress.

## Screenshots & Videos

*(Screenshot section)*

![Picture1](https://github.com/user-attachments/assets/93d075e9-721f-4cb1-bf14-d67ebf609786)
![Picture2](https://github.com/user-attachments/assets/1e7bf7d9-26c7-4f40-8f49-37a9dc2df574)
![Picture3](https://github.com/user-attachments/assets/bacf4538-40b4-4f65-bac8-08c6a563de0b)
![Picture4](https://github.com/user-attachments/assets/76b465ba-894e-47cf-b174-f6ebbdf982e3)
![Picture5](https://github.com/user-attachments/assets/4955ea22-0c2f-4be1-90fd-7abeca686565)
![Picture6](https://github.com/user-attachments/assets/52ecfa64-3791-461b-b205-62335a808ea1)
![Picture7](https://github.com/user-attachments/assets/b78455ed-7555-4cbe-8231-d4d7e688dc8a)
![Picture8](https://github.com/user-attachments/assets/d53b6e5c-8b4d-4dcc-b7ff-d343e92718ef)
![Picture9](https://github.com/user-attachments/assets/208b3301-103e-437d-9f9e-818f667d562e)

*(Video section)*

https://github.com/user-attachments/assets/eb24bd18-153c-400a-8219-3bf3df2e44f9

https://github.com/user-attachments/assets/ec2b7559-4bae-4703-b11e-de85f949c9ba

## Getting Started

### Prerequisites

To run this project, you will need:

- Visual Studio with Xamarin support
- An Android emulator, or a physical android device connected to your development environment

*(Note: Android support only. Project itself is build on Xamarin.Forms, so technically code can be easily recompiled for iOS use)*

*(Note: Project files removed from repository to make sure distribution is not possible in any way or form)*

### ~~Installation~~ (Not possible because the content has been deleted to protect against distribution)

1. Clone this repository:

   ```bash
   git clone https://github.com/dirDahsul/Tetris-Clone.git

2. Open the solution file (.sln) in Visual Studio.
3. Restore the NuGet packages if prompted.
4. Deploy the application to your desired platform (Android) by selecting the appropriate emulator or connected device.
5. Build and run the project.

### How to Play

- Classic Mode: Play without any time constraints and aim to achieve the highest score possible. The game ends when the field is full and no more tetrominoes can be placed.
- Time-Limited Mode: Score as many points as possible within 3 minutes. The game ends when time runs out or the field is full.
- Controls: Use touch gestures to move and rotate the tetrominoes. Sensitivity can be adjusted in the settings.

## Project Structure

Due to the rapid development approach, this project does not adhere to the MVVM pattern or other best practices for Xamarin development. The focus was on implementing core functionality quickly. While this results in a playable game, it suffers from optimization and code structure issues.

### Main Pages

- MainPage.xaml: The landing page with options to start the game or navigate to other sections.
- GamePage.xaml: The main gameplay interface.
- SettingsPage.xaml: Interface for configuring game settings.
- ProfilePage.xaml: Displays user profile information.
- ChallengesPage.xaml: Lists daily challenges.

## Future Improvements

Although this project was primarily for learning purposes, here are a few areas where it could be improved (future improvements are not planned):

- Refactor to implement the MVVM pattern for better separation of concerns.
- Optimize the codebase for better performance and maintainability.
- Add more detailed UI scaling and responsive design adjustments.
- Implement additional gameplay features like leaderboards or multiplayer support.
- Improve animations and overall user experience.

## Learning Journey

### Motivation
This project was my first attempt at building a mobile application using Xamarin. As a fan of classic block puzzle games, I decided to create a similar game as a way to dive into the world of mobile app development. The goal was to build a functional game as quickly as possible while learning the ins and outs of the Xamarin IDE, C#, and XAML.

### Challenges Faced

- **Time Constraints:** One of my primary goals was to complete the project rapidly, which required me to prioritize core functionality over optimal design patterns like MVVM. This was a significant challenge because it forced me to balance speed with maintaining a functional and scalable codebase.
  
- **UI Scalability:** Ensuring that the game's user interface scaled appropriately across various device sizes was a complex task. I had to experiment with different XAML layouts and Xamarin-specific features to achieve a UI that worked well on both small and large screens.
  
- **Game Logic Implementation:** Implementing the core gameplay mechanics, such as piece rotation, line clearing, and score calculation, was both challenging and rewarding. Since this was my first time working on game logic in C#, I had to invest time in understanding how to efficiently manage game state and user inputs.

- **Performance Optimization:** As the game grew more complex, I noticed performance issues, especially on lower-end devices. Sadly not enough time to implement any major performance profiling and optimization techniques within Xamarin, which was a valuable learning experience.

### Technologies Used

- **Xamarin.Forms:** The cross-platform framework used to build the UI and manage the game logic. This project helped me understand the strengths and limitations of Xamarin.Forms for game development.
  
- **C#:** The primary programming language for this project. Through this project, I deepened my understanding of C# syntax, object-oriented principles, and game development concepts.
  
- **XAML:** Used extensively for designing the UI. Working with XAML taught me how to effectively use data binding, layouts, and custom controls within Xamarin applications.

- **Visual Studio:** The integrated development environment (IDE) where I wrote, debugged, and tested the code. I became more proficient with Visual Studio's debugging tools, which were crucial in diagnosing issues during development.

### Lessons Learned

- **Rapid Prototyping:** This project taught me the value of rapid prototyping. By focusing on building a functional game quickly, I was able to iterate on the design and gameplay mechanics faster. However, it also highlighted the trade-offs between speed and code quality, especially when not using best practices like MVVM.

- **Importance of UI/UX Design:** Ensuring a smooth user experience across various devices was more challenging than expected. I learned the importance of designing flexible UI layouts that can adapt to different screen sizes and orientations.

- **Game Development Fundamentals:** Building the game logic from scratch deepened my understanding of fundamental game development concepts, such as collision detection, game state management, and user input handling.

### Future Directions

This project was a foundational step in my journey as a developer. While the game is fully functional, I recognize areas for improvement, such as refactoring the code to follow the MVVM pattern, further optimizing performance, and exploring more advanced game development frameworks. This experience has equipped me with the skills and confidence to tackle more complex projects in the future.

---

This project represents not just a game, but a comprehensive learning experience that has significantly shaped my approach to mobile development. It showcases my ability to quickly learn new technologies, overcome challenges, and deliver a functional product within a tight timeframe.

## Contributing

Since this is a personal project, that is not for distribution, I am not seeking contributions. Do not submit pull requests as they will not be approved.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgements

- Classic Tetris inspiration
- Xamarin documentation and community resources

Thank you for checking out my Tetris clone project. This was an exciting learning journey into Xamarin, and I hope it serves as a helpful reference for others starting out with mobile app development!
