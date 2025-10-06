
using Microsoft.Extensions.ML;

namespace AIWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddPredictionEnginePool<PredictiveModel.ModelInput, PredictiveModel.ModelOutput>()
    .FromFile("PredictiveModel.mlnet");
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Add this before builder.Build();
            builder.Services.AddControllers();            

            app.MapControllers();

            app.Run();
        }
    }
}
