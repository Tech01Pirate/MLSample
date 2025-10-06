# Combined README: AIWebApp & MLSampleAppConsole

This repository contains two .NET 8 projects demonstrating machine learning and content safety integration:
- **AIWebApp**: ASP.NET Core Web API for ML.NET model predictions
- **MLSampleAppConsole**: Console app for Azure Content Safety API moderation

---

## AIWebApp

AIWebApp is an ASP.NET Core Web API project that exposes machine learning model predictions via REST endpoints. It leverages ML.NET for model consumption and provides a simple interface for integrating predictive analytics into your applications.

### Features
- REST API for ML Predictions
- Swagger/OpenAPI Integration
- Prediction Engine Pooling
- Extensible Controller

### Project Structure
- `Program.cs`: Configures the web application, registers ML.NET services, and sets up Swagger.
- `Controllers/MachineAnalyzerController.cs`: Main API controller exposing the `/MachineAnalyzer/predict` endpoint for model inference.
- `PredictiveModel.*.cs`: Model input/output classes and logic for consumption, training, and evaluation.
- `PredictiveModel.mlnet`: The trained ML.NET model file (referenced in code, not included by default).
- `data.csv`: Example data file (if present) for model training or evaluation.

### Getting Started
1. Clone the repository
2. Restore NuGet packages: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the API: `dotnet run --project AIWebApp/AIWebApp.csproj`
5. Access Swagger UI at `https://localhost:5001/swagger`

### API Usage
- **Endpoint**: `POST /MachineAnalyzer/predict`
- **Request Body**: JSON matching `PredictiveModel.ModelInput`
- **Response**: JSON matching `PredictiveModel.ModelOutput`

### Model Integration
- Loads ML.NET model from `PredictiveModel.mlnet` at startup.
- Update the model file as needed.

### Extending the API
- Add controllers/endpoints in `Controllers`.
- Use DI for prediction engine pool.

### Dependencies
- Microsoft.Extensions.ML
- Microsoft.ML
- Swashbuckle.AspNetCore
- Microsoft.ML.LightGbm

---

## MLSampleAppConsole

MLSampleAppConsole is a .NET 8 console application that demonstrates how to use Azure Content Safety APIs to analyze and moderate user-provided text content. The app allows users to input text, detects potentially unsafe content across several categories (Hate, SelfHarm, Sexual, Violence), and makes automated decisions based on configurable thresholds.

### Features
- Interactive Console Input
- Content Safety Detection
- Configurable Thresholds
- Automated Decision Making
- Extensible Model

### Requirements
- .NET 8 SDK
- Azure Content Safety endpoint and subscription key

### Getting Started
1. Clone the repository
2. Configure Azure credentials in `Programm.cs`
3. Build the project: `dotnet build`
4. Run the app: `dotnet run`

### Usage
- Enter text when prompted.
- App analyzes and displays Accept/Reject decision.

### Example
```
---------------------------------
Enter the content to be Tested.
> This is a test message.
Decision: Accept
```

### Project Structure
- `Programm.cs`: Main logic and entry point.
- `Models/DetectionResult.cs`: Detection result model.
- `Models/DetectionException.cs`: Exception handling.
- `Enums.cs`: Media types, categories, actions.

### Extending
- Add categories to `Enums.cs`.
- Modify threshold logic in `Programm.cs`.
- Extend models in `Models` for more analysis.

---

## License
This repository is for demonstration purposes. Please update with your own license as needed.

## Contact
For questions or support, please contact the project maintainer.
