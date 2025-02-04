using Contracts;
using MassTransit;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features;

public sealed class ArticleViewedConsumer(ApplicationDbContext dbContext) : IConsumer<ArticleViewedEvent>
{
    public Task Consume(ConsumeContext<ArticleViewedEvent> context)
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
            ArticleId = context.Message.Id,
            
        };
    }
}