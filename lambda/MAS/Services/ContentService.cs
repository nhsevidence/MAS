﻿using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IContentService
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime? date = null);
    }

    public class ContentService : IContentService
    {
        private readonly ILogger<ContentService> _logger;

        public ContentService(ILogger<ContentService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI));
                    var item = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                    return item;
                }
                catch(Exception e)
                {
                    _logger.LogError($"Failed to get items from CMS - exception: {e.Message}");
                    throw new Exception($"Failed to get items from CMS - exception: {e.Message}");
                }
            }
        }

        public async Task<IEnumerable<Item>> GetDailyItemsAsync(DateTime? date = null)
        {
            date = date ?? DateTime.Today;

            using (WebClient client = new WebClient())
            {
                try
                {
                    var jsonStr = await client.DownloadStringTaskAsync(new Uri(AppSettings.CMSConfig.URI)); //TODO Use date
                    var item = JsonConvert.DeserializeObject<Item[]>(jsonStr);
                    return item;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to get items from CMS - exception: {e.Message}");
                    throw new Exception($"Failed to get items from CMS - exception: {e.Message}");
                }
            }
        }

    }
}
