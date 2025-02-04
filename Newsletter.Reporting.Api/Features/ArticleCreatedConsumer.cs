using Contracts;
using MassTransit;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Features;

public sealed class ArticleCreatedConsumer(ApplicationDbContext dbContext) : IConsumer<ArticleCreatedEvent>
{
    public async Task Consume(ConsumeContext<ArticleCreatedEvent> context)
    {
        var article = new Article
        {
            Id = context.Message.Id,
            CreatedOnUtc = context.Message.CreatedOnUtc
        };
        
        await dbContext.AddAsync(article);

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}