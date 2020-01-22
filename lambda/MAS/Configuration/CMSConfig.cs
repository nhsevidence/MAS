﻿namespace MAS.Configuration
{
    public class CMSConfig
    {
        public static CMSConfig Current { get; private set; }

        public CMSConfig()
        {
            Current = this;
        }

        public string BaseUrl { get; set; }
        public string AllItemsPath { get; set; }
        public string DailyItemsPath { get; set; }
    }
}
