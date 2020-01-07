﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MAS.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAS.Controllers
{
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IContentService _contentService;

        public MailController(IMailService mailService, IContentService contentService)
        {
            _mailService = mailService;
            _contentService = contentService;
        }

        //PUT api/mail/daily
        [HttpPut("daily")]
        public async Task<IActionResult> PutMailAsync()
        {
            var items = await _contentService.GetItemsAsync();

            var body = _mailService.CreateEmailBody(items);
            var subject = "MAS Email";
            var previewText = "This MAS email was created " + DateTime.Now.ToShortDateString();

            try
            {
                var campaignId = await _mailService.CreateAndSendCampaignAsync(subject, previewText, body);
                return Content(campaignId);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = e.Message, Detail = e.InnerException?.Message });
            }
        }
    }
}