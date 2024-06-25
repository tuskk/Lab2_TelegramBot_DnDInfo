# D&D Telegram Bot

This is a Telegram bot that provides information about Dungeons & Dragons (D&D) classes, spells, and allows you to roll dice.

## Features

- List available D&D classes
- Get details and pictures of each D&D class
- Get details of a specific spell
- Roll dice using a dice expression

## Usage

1. Start a chat with the bot by searching for it on Telegram: `@DnD_Bot`
2. Use the following commands to interact with the bot:

- `/start`: Get a welcome message and see available commands
- `/help`: List all available commands
- `/classes`: List all available D&D classes
- `/{class}`: Get details and a picture of a specific D&D class (e.g. `/barbarian`)
- `/spell [spell name]`: Get details of a specific spell (e.g. `/spell fireball`)
- `/roll [dice expression]`: Roll the specified dice (e.g. `/roll 2d6`)

## Development

To run the bot locally, follow these steps:

1. Clone the repository: `git clone https://github.com/your-username/DnDTelegramBot.git`
2. Open the project in Visual Studio
3. Replace the `botToken` variable in the `Program.cs` file with your own Telegram bot token
4. Build and run the project

## Dependencies

- Telegram.Bot: Library for interacting with the Telegram Bot API
- Newtonsoft.Json: Library for working with JSON data
