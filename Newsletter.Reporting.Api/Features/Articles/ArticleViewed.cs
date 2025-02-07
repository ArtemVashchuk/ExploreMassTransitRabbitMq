using Contracts;
using MassTransit;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features.Articles;

public sealed class ArticleViewedConsumer(ApplicationDbContext dbContext) : IConsumer<ArticleViewedEvent>
{
    public async Task Consume(ConsumeContext<ArticleViewedEvent> context)
    {
        var article = dbContext
            .Articles
                .FirstOrDefault(a => a.Id == context.Message.Id);

        if (article is null)
        {
            return;
        }

        var articleEvent = new ArticleEvent
        {
            Id = Guid.NewGuid(),
            ArticleId = article.Id,
            CreatedOnUtc = context.Message.ViewedOnUtc,
            EventType = ArticleEventType.View
        };

        await dbContext.AddAsync(articleEvent);
        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}