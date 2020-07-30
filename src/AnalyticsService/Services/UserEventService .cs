using System.Collections.Generic;
using System.Threading.Tasks;
using AnalyticsService.Contexts;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AnalyticsService.Services
{
    public interface IUserEventService
    {
        public Task<List<UserEvent>> Get();
        public Task<UserEvent> Create(UserEvent userEvent);
    }

    public class UserEventService : IUserEventService
    {
        private readonly IAnalyticsContext _analyticsContext;

        public UserEventService(IAnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        
        public async Task<List<UserEvent>> Get()
        {
            return await _analyticsContext
                .UserEvents
                .Find(p => true)
                .ToListAsync();
        }
        
        public async Task<UserEvent> Create(UserEvent userEvent)
        {
            await _analyticsContext.UserEvents.InsertOneAsync(userEvent);
            return userEvent;
        }
    }
}