using System.Collections.Generic;
using System.Threading.Tasks;
using AnalyticsService.Contexts;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AnalyticsService.Services
{
    public interface IAuthEventService
    {
        public Task<List<AuthEvent>> Get();
        public Task<AuthEvent> Create(AuthEvent authEvent);
    }

    public class AuthEventService : IAuthEventService
    {
        private readonly IAnalyticsContext _analyticsContext;

        public AuthEventService(IAnalyticsContext analyticsContext)
        {
            _analyticsContext = analyticsContext;
        }
        
        public async Task<List<AuthEvent>> Get()
        {
            return await _analyticsContext
                .AuthEvents
                .Find(p => true)
                .ToListAsync();
        }
        
        public async Task<AuthEvent> Create(AuthEvent authEvent)
        {
            await _analyticsContext.AuthEvents.InsertOneAsync(authEvent);
            return authEvent;
        }
    }
}