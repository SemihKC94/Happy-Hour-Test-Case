# Word Puzzle Game - README

This project is a word puzzle game developed using Unity 2022.3.27f1 LTS. The game allows users to find words in a grid of letters, with word lists loaded from JSON files.

# By clicking on the logo below, you can see the Gameplay video.

[![Watch the GamePlay](https://iili.io/3wTYRuS.png)](https://youtube.com/shorts/NrxXCXHh4OY)

## Key Features

* Configurable grid size.
* Word validation against a JSON word list.
* Visual feedback for word selection.
* Trie-optimized word search.

## Architecture and Design

###   SOLID Principles

* **Single Responsibility Principle (SRP):**
    * `GridCell` is responsible for managing a single cell in the grid (displaying the letter, handling selection).
    * `GridManager` handles the grid logic (creation, word finding).
    * `WordManager` (not provided, but assumed) is responsible for word list management and validation.
    * `Trie` is solely responsible for efficient word storage and prefix searching.
    * `EventBroker` is responsible for communication between different parts of the game.
    * `GameManager` is responsible for game flow.

* **Open/Closed Principle (OCP):**
    * The event-driven system using `EventBroker` allows for adding new game logic without modifying existing classes significantly. For example, new actions can be added to respond to `OnWordChange` without altering `GridManager`.
    * `GridManager` can work with different data sources if the `WordManager` is abstracted correctly (e.g., different word list formats).
 
* **Interface Segregation Principle (ISP):**
    * The code uses Actions (`System.Action`, `System.Action<string>`) for event handling, which can be seen as a form of interface segregation. Each event defines the specific methods that subscribers need to implement.
 
* **Dependency Inversion Principle (DIP):**
    * The `EventBroker` promotes DIP by decoupling classes that need to communicate. Instead of direct dependencies, classes publish and subscribe to events. For example, `GridManager` doesn't need to know which other classes react to word selection events.
    * `GridManager` depends on an abstraction (`WordManager`) rather than a concrete implementation for word validation.

###   OOP Principles

* **Encapsulation:**
    * `GridCell` encapsulates its state (`Letter`, `Row`, `Col`, selection state) and behavior (selection, deselection).
    * `Trie` encapsulates the word storage and search logic.
    
* **Abstraction:**
    * The `GridCell` provides an abstraction of a single cell in the game grid, hiding the underlying rendering and interaction details.
    * The `EventBroker` abstracts the communication mechanism between different parts of the game.
      
* **Inheritance:**
    * Minimal use of inheritance

###   Design Patterns

* **Event Broker:**
    * The `EventBroker` class implements the Event Broker pattern. This pattern facilitates communication between different parts of the system without requiring them to have direct dependencies on each other. This promotes loose coupling and makes the system more flexible and maintainable.
    * **Example:** `GridManager` publishes events like `OnWordChange` and `OnFoundWord`, while other parts of the game (e.g., UI elements) subscribe to these events to react accordingly.
      
* **Factory Pattern:**
    * The `GridManager.CreateGrid` method can be seen as a simple Factory, responsible for creating instances of `GridCell` and initializing them.
      
* **Object Pool:**
    * The `Helper.WaitDictionary` is a form of object pooling for `WaitForSeconds` instances, improving performance by reusing instances instead of allocating new ones repeatedly.

###   Trie Algorithm

* **Why use Trie?**
    * The Trie data structure is used in the `WordManager` to efficiently store and search for words. This is crucial for optimizing the word search functionality, especially when dealing with a large word list.
    * As highlighted in the PDF, using Trie is recommended for optimizing word search algorithms.
      
* **Advantages of Trie:**
    * **Efficient Prefix Searching:** Trie allows for very fast prefix-based searches. In the context of the game, this means we can quickly determine if a sequence of selected letters is a valid prefix of any word in the dictionary, significantly reducing the search space.
    * **Fast Word Validation:** Checking if a complete word is valid is also efficient, as it involves traversing the Trie along the path representing the word.
    * **Space Efficiency (Potentially):** For a large set of words with common prefixes, a Trie can be more space-efficient than storing each word separately.
      
* **Implementation:**
    * The `TrieNode` class represents a node in the Trie, storing child nodes for each character and a flag to indicate the end of a word.
    * The `Trie` class provides methods to `Insert` words, check if a word `IsWord`, and check if a string is a `IsPrefix`.

This README provides a solid foundation for understanding the project's architecture and design choices.
