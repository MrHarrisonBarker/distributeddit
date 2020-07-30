using System.Collections.Generic;
using System.Threading.Tasks;
using AnalyticsService.Contexts;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AnalyticsService.Services
{
    public interface IPostEventService
    {
        public Task<List<PostEvent>> Get();
        public Task<PostEvent> Create(PostEvent postEvent);
    }

    public class PostEventService : IPostEventService
    {
        private readonly IAnalyticsContext _analyticsContext;

        public PostEventService(IAnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }

        public async Task<List<PostEvent>> Get()
        {
            return await _analyticsContext
                .PostEvents
                .Find(p => true)
                .ToListAsync();
        }

        public async Task<PostEvent> Create(PostEvent postEvent)
        {
            await _analyticsContext.PostEvents.InsertOneAsync(postEvent);
            return postEvent;
        }
    }
}