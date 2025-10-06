using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

namespace AIWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class MachineAnalyzerController : ControllerBase
    {
        private readonly PredictionEnginePool<PredictiveModel.ModelInput, PredictiveModel.ModelOutput> _predictionEnginePool;

        public MachineAnalyzerController(PredictionEnginePool<PredictiveModel.ModelInput, PredictiveModel.ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpPost]
        [Route("predict")]
        public ActionResult<PredictiveModel.ModelOutput> Predict([FromBody] PredictiveModel.ModelInput input)
        {
            var result = _predictionEnginePool.Predict(input);
            return Ok(result);
        }
    }
}
