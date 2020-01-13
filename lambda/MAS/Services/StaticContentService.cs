﻿using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IStaticContentService
    {
        Task<PutObjectResponse> Write(string slug, string body);
    }
    public class S3StaticContentService : IStaticContentService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3StaticContentService> _logger;

        public S3StaticContentService(IAmazonS3 amazonS3, ILogger<S3StaticContentService> logger)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
        }

        public async Task<PutObjectResponse> Write(string slug, string body)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = AppSettings.AWSConfig.BucketName,
                Key = slug + ".html",
                ContentBody = body
            };

            try
            {
                var response = await _amazonS3.PutObjectAsync(request);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to write to S3 - exception: {e.Message}");
                throw new Exception($"Failed to write to S3 - exception: {e.Message}");
            }
            
        }
    }
}