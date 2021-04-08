using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderTracker.Imp
{
    public class SpiderRunningCache
    {
        public SpiderRunningCache(string category, string site, string userId)
        {
            this.Category = category;
            this.Site = site;
            this.CurrentUserId = userId;

            if (!string.IsNullOrEmpty(userId))
            {
                EnabledUserCahce = true;

                this.ExistsImageLocalFiles = PathUtil.GetStoreUserThumbnailImageFiles(category, userId);
                this.ExistsVideoLocalFiles = PathUtil.GetStoreUserVideoFiles(category, userId);
            }
            else
            {
                EnabledUserCahce = false;
            }
        }

        public string CurrentUserId { get; set; }

        public string Category { get; set; }

        public string Site { get; set; }

        public bool EnabledUserCahce { get; set; }

        public int CurrentPageIndex { get; set; } = 1;

        public string[] ExistsImageLocalFiles { get; set; } = new string[] { };

        public string[] ExistsVideoLocalFiles { get; set; } = new string[] { };
    }
}
