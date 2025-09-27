using FIAPCloudGames.API.Extensions;
using FIAPCloudGames.Application.Requests;
using FIAPCloudGames.Application.Responses;
using FIAPCloudGames.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FIAPCloudGames.API.Endpoints;

public static class GameEndpoints
{

    public static WebApplication MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        //Um método só para gerar erros e poder captar nos logs
        group.MapGet("/random-errors", () =>
        {
            var random = new Random();
            int randomValue = random.Next(0, 20);
            if (randomValue >= 0 && randomValue <= 5)
                return Results.UnprocessableEntity();
            else if (randomValue > 5 && randomValue <= 10)
                return Results.Unauthorized();
            else if (randomValue > 10 && randomValue <= 15)
                return Results.InternalServerError();
            else
                return Results.Conflict();
        }).AllowAnonymous();

        group.MapGet("/", async (IGameService service, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => 
        {

            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 1;

            if (pageSize > 100)
                pageSize = 100;

            int skip = (page - 1) * pageSize;

            var games = await service.FindAll(skip: skip, take: pageSize);

            return Results.Ok(games?.Select(item => new GetGameResponse { 
                Id = item.Id, 
                Name = item.Name,
                Description = item.Description,
                CategoryDescription = item.Category.GetDescription(),
                LastUpdate = item.LastUpdate,
                Price = item.Price,
                ReleaseDate = item.ReleaseDate
            }));
        }).AllowAnonymous();

        group.MapGet("/{id:guid}", async (IGameService service, [FromRoute] Guid id) => {
            var game = await service.Find(id: id);

            if (game == null)
                return Results.NotFound();

            return Results.Ok(new GetGameResponse
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                CategoryDescription = game.Category.GetDescription(),
                LastUpdate = game.LastUpdate,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate
            });
        }).AllowAnonymous();

        group.MapDelete("/{id:guid}", async (IGameService service, [FromRoute] Guid id) => {
            await service.Delete(id: id);

            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");

        group.MapPost("/", async (IGameService service, [FromBody] CreateGameRequest request) =>
        {
            if (request == null)
                return Results.BadRequest(new GenericMessageResponse { Message = "Invalid body" });

            Guid id = await service.Create(new Domain.ViewModels.GameViewModel {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = request.Category,
                ReleaseDate = request.ReleaseDate.ToUniversalTime()
            });

            return Results.Ok(new { Id = id });
        }).RequireAuthorization("AdminOnly");

        group.MapPatch("/{id:guid}", async (IGameService service, [FromRoute] Guid id, [FromBody] UpdateGameRequest request) => {
            if (request == null)
                return Results.BadRequest(new GenericMessageResponse { Message = "Invalid body" });

            var gameFound = await service.Find(id: id);
            if (gameFound == null)
                return Results.NotFound();

            gameFound.ToEntity().Update(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                category: request.Category,
                releaseDate: request.ReleaseDate?.ToUniversalTime()
            );

            await service.Update(gameFound);

            return Results.Ok();
        }).RequireAuthorization("AdminOnly");

        group.MapGet("/metrics/by-category", async (IGameService service) =>
        {
            var metrics = await service.GetCategoryMetrics();

            var list = new List<CategoryMetric>();
            if (string.IsNullOrWhiteSpace(metrics))
                list = new List<CategoryMetric>();

            var response = JsonConvert.DeserializeObject<CategoryMetricResponse>(metrics);
            list = response?.AggregationResult?.Buckets ?? new List<CategoryMetric>();

            return Results.Ok(list);
        }).AllowAnonymous();


        return app;
    }
}
