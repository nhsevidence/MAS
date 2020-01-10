﻿using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using MAS.Configuration;
using MAS.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MAS.Services
{
    public interface IS3Service
    {
        Task<PutObjectResponse> WriteToS3(Item item);
    }
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IAmazonS3 amazonS3, ILogger<S3Service> logger)
        {
            _amazonS3 = amazonS3;
            _logger = logger;
        }

        public async Task<PutObjectResponse> WriteToS3(Item item)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = AppSettings.AWSConfig.BucketName,
                Key = item.Id + ".txt",
                ContentBody = CreateContentBody(item)
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

        private string CreateContentBody(Item item)
        {
            var contentBody = new StringBuilder();
            contentBody.Append("Title: ");
            contentBody.Append(item.Title);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Short Summary: ");
            contentBody.Append(item.ShortSummary);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Source: ");
            contentBody.Append(item.Source.Title);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("Evidence Type: ");
            contentBody.Append(item.EvidenceType.Title);
            contentBody.Append(Environment.NewLine);

            contentBody.Append("UKMI Comment: ");
            contentBody.Append(item.Comment);
            contentBody.Append(Environment.NewLine);

            if (item.ResourceLinks != null)
            {
                contentBody.Append("Resource Links: ");
                contentBody.Append(item.ResourceLinks);
            }

            return contentBody.ToString();
        }
    }
}
