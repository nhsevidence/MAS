﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Configuration
{
    public class MailConfig
    {
        public string ApiKey { get; set; }
        public string ListId { get; set; }
        public int DailyTemplateId { get; set; }
        public string CampaignFolderId { get; set; }
    }
}
