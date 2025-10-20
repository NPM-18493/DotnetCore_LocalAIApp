# Local AI Chat App

A complete full-stack AI chat application featuring a .NET Web API backend and a React frontend, inspired by GitHub Copilot's interface design.

## 🌟 Overview

This project demonstrates how to build a modern AI chat application using Microsoft's AI framework with Ollama for local AI model hosting. The solution consists of two main components:

- **LocalAIAppAPI**: ASP.NET Core Web API that interfaces with Ollama
- **local-ai-app-client**: Next.js React frontend with a Copilot-inspired UI

**Reference**: Code inspired by [Microsoft Learn - Chat with Local Models](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/chat-local-model)


## 📋 Prerequisites

- **.NET 9.0 SDK** or later
- **Node.js 18+** and npm
- **Ollama** installed and running
- **phi3:mini model** (or your preferred model)

### Setting up Ollama

1. **Install Ollama**: Download from [https://ollama.com/](https://ollama.com/)
2. **Pull the model**:
   ```bash
   ollama pull phi3:mini
   ```
3. **Verify installation**:
   ```bash
   ollama list
   ```
### Front End

It's a simple next.js app sending API call with prompt message to backend API

### Back End

It's a .net core web API which gets response for the prompt from locally installed ollama model











