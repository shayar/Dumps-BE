# DUMPS Backend Project

This is the backend application for our Dumps Selling Site. The backend is built using .NET Core and follows the principles of Clean Architecture.

## Table of Contents

- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Branching Strategy](#branching-strategy)
- [Development Workflow](#development-workflow)
- [Running the Project](#running-the-project)
- [Testing](#testing)
- [Code Quality](#code-quality)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

To get started with the project, follow these steps:

1. **Clone the repository:**
   ```bash
   git clone git@github.com:shayar/Dumps-BE.git
   cd Dumps-BE
   ```
## Run Locally

2. **Install PostgreSQL**
    - If you haven't already, install PostgreSQL on your machine. You can find installation instructions on the [PostgreSQL official website](https://www.postgresql.org/download/).

3. **Add user in PostgreSQL**
   ```bash
    CREATE ROLE dumps_user WITH LOGIN PASSWORD 'dumps_pass' CREATEDB;
    ```
4. **Install dependencies:**
   ```bash
   dotnet restore
   ```

5. **Run the development server:**
   ```bash
   dotnet run
   ```

This will start the development server on `http://localhost:8080`.

## Run Using Docker
2. **Install Docker**
    - If you haven't already, install Docker on your machine. You can find installation instructions on the [Docker official website](https://docs.docker.com/get-started/get-docker/).

3. **Run docker**
    ```bash
   docker-compose up --build 
   ```

This will start the development server on `http://localhost:8080`.

## Project Structure

The project is structured according to the principles of Clean Architecture, ensuring a clear separation of concerns and maintainability.

```
backend/
│
├── src/
│   ├── Core/                    # Domain layer: Entities, Interfaces, Specifications
│   ├── Application/             # Application layer: Use Cases, DTOs, Services
│   ├── Infrastructure/          # Infrastructure layer: Data access, External services
│   ├── Web/                     # Presentation layer: Controllers, View Models, API
│   └── Tests/                   # Test projects for each layer
│
├── .env                         # Environment variables
├── .gitignore                   # Git ignore rules
├── README.md                    # Project documentation
└── Backend.sln                  # Solution file
```

### Clean Architecture Layers

- **Core**: The domain layer containing business entities, interfaces, and domain logic.
- **Application**: The application layer implementing use cases, DTOs, and application services.
- **Infrastructure**: The infrastructure layer handling data persistence, external APIs, and other I/O concerns.
- **Web**: The presentation layer, mainly the API controllers, handling HTTP requests and responses.

## Branching Strategy

We follow a structured branching strategy to ensure smooth development and deployment processes:

- **Main Branch (`main`)**: 
  - This is the master branch and should always contain the production-ready code.
  
- **Release Branch (`release`)**: 
  - This branch is used for the preparation of production releases. It contains stable code ready for production deployment.
  
- **Sprint Branches (`sprint-x`)**:
  - A sprint branch is created from the `release` branch for each sprint. All feature branches are created from the respective sprint branch.
  
- **Feature Branches**:
  - Feature branches are created from the current sprint branch. The name of the branch should correspond to the JIRA task ID (e.g., `ID-10`).

### Branching Workflow

1. **Pull the `release` branch:**
   ```bash
   git checkout release
   git pull origin release
   ```

2. **Create a new sprint branch from `release`:**
   ```bash
   git checkout -b sprint-x
   ```

3. **Create a feature branch from the sprint branch:**
   ```bash
   git checkout -b ID-10
   ```
   (Replace `ID-10` with your actual JIRA task ID.)

4. **Work on your feature, commit changes, and push the branch:**
   ```bash
   git add .
   git commit -m "ID-10: Implemented feature XYZ"
   git push origin ID-10
   ```

5. **Before pushing your final changes, pull the latest changes from the sprint branch:**
   ```bash
   git checkout sprint-x
   git pull origin sprint-x
   git checkout ID-10
   git merge sprint-x
   ```

6. **Push your feature branch and create a merge request into the sprint branch:**
   ```bash
   git push origin ID-10
   ```

7. **Sprint Closure:**
   - After all features are merged into the sprint branch, create a pull request from the sprint branch to the `release` branch and merge it.
   - Deploy the code from the `release` branch to production.

8. **Post-Release:**
   - Once the code is verified in production, create a pull request from the `release` branch to the `main` branch and merge it.

## Development Workflow

1. **Feature Development**: 
   - Start a new branch for each feature or bugfix using the JIRA task ID as the branch name.
   
2. **Commit Messages**:
   - Follow the format: `ID-XX: Your commit message`, where `ID-XX` is the JIRA task ID.

3. **Pull Requests**:
   - Open a pull request against the sprint branch when your feature is complete.
   - Make sure all tests pass before requesting a review.

4. **Code Reviews**:
   - All pull requests require at least one approval before merging.

5. **Merge Requests**:
   - Merge your feature branch into the sprint branch once it has been reviewed and approved.
   - Merge the sprint branch into `release` after the sprint is completed.

## Running the Project

To start the project in development mode:

```bash
dotnet run
```

To build the project for production:

```bash
dotnet build -c Release
```

## Testing

To run the tests:

```bash
dotnet test
```

Ensure that all tests pass before submitting a pull request.

## Code Quality

- **Static Analysis**: 
  - We use tools like SonarQube or ReSharper to enforce code quality standards. Run the analysis using:
    ```bash
    dotnet sonarscanner begin /k:"project-key"
    dotnet build
    dotnet sonarscanner end
    ```

- **Formatting**:
  - Code formatting is enforced using a .NET code formatter. Format your code with:
    ```bash
    dotnet format
    ```

## Contributing

Please follow the [contributing guidelines](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

This README file outlines the key aspects of the backend project, including the branching strategy, workflow, and instructions for running and contributing to the project, while adhering to Clean Architecture principles. It serves as a guide for developers to maintain consistency and quality throughout the development process.
