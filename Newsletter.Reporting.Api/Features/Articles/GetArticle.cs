using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Shared;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features.Articles;

public static class GetArticle
{
    public record Query : IRequest<Result<ArticleResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, Result<ArticleResponse>>
    {
        public async Task<Result<ArticleResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var articleResponse = await dbContext
                .Articles
                .AsNoTracking()
                .Where(article => article.Id == request.Id)
                .Select(article => new ArticleResponse
                {
                    Id = article.Id,
                    CreatedOnUtc = article.CreatedOnUtc,
                    PublishedOnUtc = article.PublishedOnUtc,
                    Events = dbContext.ArticleEvents
                        .Where(articleEvent => articleEvent.ArticleId == article.Id)
                        .Select(articleEvent => new ArticleEventResponse
                        {
                            Id = articleEvent.Id,
                            CreatedOnUtc = articleEvent.CreatedOnUtc,
                            EventType = articleEvent.EventType
                        }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (articleResponse is null)
            {
                return Result.Failure<ArticleResponse>(new Error(
                    "GetArticle.Null",
                    "The article with the specified ID was not found"));
            }

            return articleResponse;
        }
    }
}

public record GetArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/article/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetArticle.Query { Id = id };
            var result = await sender.Send(query);

            if (result.IsFailure)
                return Results.NotFound(result.Error);

            return Results.Ok(result.Value);
        });
    }
}

public record ArticleResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? PublishedOnUtc { get; set; }
    public List<ArticleEventResponse> Events { get; set; } = new();
}

public record ArticleEventResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public ArticleEventType EventType { get; set; }
}