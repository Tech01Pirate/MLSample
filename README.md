# AIWebApp

AIWebApp is an ASP.NET Core Web API project that exposes machine learning model predictions via REST endpoints. It leverages ML.NET for model consumption and provides a simple interface for integrating predictive analytics into your applications.

## Features
- **REST API for ML Predictions**: Easily send data and receive predictions from a trained ML.NET model.
- **Swagger/OpenAPI Integration**: Interactive API documentation and testing via Swagger UI.
- **Prediction Engine Pooling**: Efficient, thread-safe model serving using `PredictionEnginePool`.
- **Extensible Controller**: Example controller for custom endpoints and business logic.

## Project Structure
- `Program.cs`: Configures the web application, registers ML.NET services, and sets up Swagger.
- `Controllers/MachineAnalyzerController.cs`: Main API controller exposing the `/MachineAnalyzer/predict` endpoint for model inference.
- `PredictiveModel.*.cs`: Model input/output classes and logic for consumption, training, and evaluation.
- `PredictiveModel.mlnet`: The trained ML.NET model file (referenced in code, not included by default).
- `data.csv`: Example data file (if present) for model training or evaluation.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Trained ML.NET model file (`PredictiveModel.mlnet`)

### Setup
1. **Clone the repository**
2. **Restore NuGet packages**
   ```sh
   dotnet restore
   ```
3. **Build the project**
   ```sh
   dotnet build
   ```
4. **Run the API**
   ```sh
   dotnet run --project AIWebApp/AIWebApp.csproj
   ```
5. **Access Swagger UI**
   Navigate to `https://localhost:5001/swagger` (or the port shown in your console) to explore and test the API.

## API Usage

### Predict Endpoint
- **URL**: `POST /MachineAnalyzer/predict`
- **Request Body**: JSON object matching `PredictiveModel.ModelInput` properties
- **Response**: JSON object matching `PredictiveModel.ModelOutput` properties

#### Example Request
```json
{
  "feature1": 123,
  "feature2": "value",
  // ... other model input fields
}
```

#### Example Response
```json
{
  "prediction": "Result",
  // ... other model output fields
}
```

## Model Integration
- The API loads the ML.NET model from `PredictiveModel.mlnet` at startup.
- Update the model file as needed to deploy new versions.
- Input/output classes are defined in `PredictiveModel.*.cs` files.

## Extending the API
- Add new controllers or endpoints in the `Controllers` folder.
- Use dependency injection to access the prediction engine pool in your controllers.

## Dependencies
- [Microsoft.Extensions.ML](https://www.nuget.org/packages/Microsoft.Extensions.ML)
- [Microsoft.ML](https://www.nuget.org/packages/Microsoft.ML)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore)
- [Microsoft.ML.LightGbm](https://www.nuget.org/packages/Microsoft.ML.LightGbm)

## License
This project is for demonstration purposes. Please update with your own license as needed.

## Contact
For questions or support, please contact the project maintainer.
