using FIAPCloudGames.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FIAPCloudGames.API.Endpoints;

public static class EventSourcingEndpoints
{

    public static WebApplication MapEventSourcingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/events");

        group.MapGet("/", async (IEventRepository service, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => {

            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 1;

            if (pageSize > 100)
                pageSize = 100;

            int skip = (page - 1) * pageSize;

            var events = await service.GetAll(skip, pageSize);
            return Results.Ok(events);
        }).AllowAnonymous();

        group.MapGet("/{id:guid}", async (IEventRepository service, [FromRoute] Guid id) => {
            var data = await service.GetAllByAggregateId(aggregateId: id);

            if (data == null)
                return Results.NotFound();

            return Results.Ok(data);
        }).AllowAnonymous();

        return app;
    }
}
