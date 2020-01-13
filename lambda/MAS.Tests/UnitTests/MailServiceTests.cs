﻿using MAS.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;
using MAS.Services;
using MailChimp.Net.Interfaces;
using Moq;
using MailChimp.Net.Models;
using MailChimp.Net.Core;
using Shouldly;
using Microsoft.Extensions.Logging;
using System;

namespace MAS.Tests.UnitTests
{
    public class MailServiceTests : TestBase
    {
        [Fact]
        public void CreateCampaignAndSendToMailChimp()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            mockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            mockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>()));

            var mailService = new MailService(mockMailChimpManager.Object, mockLogger.Object);

            //Act
            var response = mailService.CreateAndSendDailyCampaignAsync("Test Subject", "Preview Text", "Body Text");
            
            //Assert
            response.Exception.ShouldBe(null);
            response.Result.ShouldBe("1234");
            response.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        [Fact]
        public void ErrorInSendingCampaignShouldThrowError()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<MailService>>();

            var mockMailChimpManager = new Mock<IMailChimpManager>();
            mockMailChimpManager.Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>())).ReturnsAsync(new Campaign() { Id = "1234" });
            mockMailChimpManager.Setup(x => x.Content.AddOrUpdateAsync(It.IsAny<string>(), It.IsAny<ContentRequest>()));
            mockMailChimpManager.Setup(x => x.Campaigns.SendAsync(It.IsAny<string>())).Throws(new Exception());

            var mailService = new MailService(mockMailChimpManager.Object, mockLogger.Object);

            //Act + Assert
            Should.Throw<Exception>(() => mailService.CreateAndSendDailyCampaignAsync("Test Subject", "Preview Text", "Body Text"));
        }
    }
}
