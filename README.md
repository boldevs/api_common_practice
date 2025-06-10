# .NET Common Practices API

This repository showcases a sample CRUD (Create, Read, Update, Delete) API for products, built using modern .NET practices. It leverages **Minimal APIs** for a leaner and faster approach compared to traditional controllers. The project is designed to be a practical example of implementing common patterns and tools in a .NET environment.

## ✨ Features

* **CRUD Operations**: Basic create, read, update, and delete functionality for products.
* **Minimal APIs**: Built with .NET 9's Minimal API framework for concise and performant endpoints.
* **Distributed Caching**: Uses **Redis** to implement distributed caching for improved performance and scalability.
* **Containerization**: Full **Docker** and **docker-compose** support for easy development and deployment.
* **CI/CD**: Includes a **GitHub Actions** workflow for continuous integration and deployment.
* **Global Error Handling**: Implements a global exception handler to manage errors gracefully.
* **Problem Details**: Uses `ProblemDetails` for standardized and detailed API error responses.
* **Structured Logging**: Integrated with **Seq** for structured and easily searchable logs.
* **API Versioning**: Demonstrates how to implement API versioning to manage changes over time.
* **Comprehensive Testing**: Includes **Unit Tests** and **Integration Tests** using **xUnit** to ensure code quality.

## 🛠️ Technologies Used

* **.NET 9**: The latest version of the .NET framework.
* **PostgreSQL**: A powerful, open-source object-relational database system.
* **Redis**: An in-memory data structure store, used as a distributed cache.
* **Docker**: A platform for developing, shipping, and running applications in containers.
* **Seq**: A centralized logging server for structured application logs.
* **xUnit**: A free, open-source, community-focused unit testing tool for the .NET Framework.
* **Minimal APIs**: A streamlined way to build HTTP APIs in ASP.NET Core.

## 📂 Project Structure

The solution is organized to separate concerns and maintain a clean architecture.

```plaintext
/
├── .github/              # GitHub Actions CI/CD workflows
├── Product.API/          # The main API project
│   ├── Data/             # Database context and migrations
│   ├── Endpoints/        # Minimal API endpoints
│   ├── Models/           # Data models and DTOs
│   ├── Services/         # Business logic and services
│   ├── Middlewares/      # Custom middleware
│   ├── Extensions/       # Service registration extensions
│   ├── appsettings.json  # Configuration
│   └── Program.cs        # Application entry point
├── Testing/              # Unit and Integration tests
├── .dockerignore         # Specifies files to ignore in Docker builds
├── docker-compose.yml    # Defines services for local development
└── Product.API.sln       # Visual Studio solution file
````



## 🚀 Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### 🛠️ Prerequisites

Make sure you have the following software installed on your system:

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) - The software development kit for building the application.
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) - Required to run the application and services using docker-compose.
* An IDE or code editor of your choice, such as:
    * [Visual Studio 2022](https://visualstudio.microsoft.com/)
    * [JetBrains Rider](https://www.jetbrains.com/rider/)
    * [Visual Studio Code](https://code.visualstudio.com/)

### ⚙️ Installation & Running

1.  **Clone the Repository**

    Open your terminal or command prompt and clone this repository to your local machine:
    ```bash
    git clone https://github.com/boldevs/api_common_practice.git
    ```

2.  **Configure Your Settings**

    Before running the application, you need to set up your local configuration:
    * Navigate to the `Product.API` directory.
    * Open the `appsettings.Development.json` file.
    * Update the `ConnectionStrings` for your **PostgreSQL** database.
    * Update the `CacheSettings` for your **Redis** instance.
    * Update the `Serilog` configuration for your **Seq** logging server.

3.  **Run the Application**

    You have two options for running the project:

    **A) With `docker-compose` (Recommended)**

    This is the simplest way to start all the necessary services (API, Database, Cache, and Logging) at once.
    ```bash
    docker-compose up --build
    ```
    The API will then be accessible at `http://localhost:5000`.

    **B) With the `.NET CLI`**

    If you prefer to manage your own database and other services, you can run the API directly.
    ```bash
    # Restore dependencies
    dotnet restore

    # Run the project
    dotnet run --project Product.API
    ```
