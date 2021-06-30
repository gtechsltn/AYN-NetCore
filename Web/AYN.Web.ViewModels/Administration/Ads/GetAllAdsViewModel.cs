﻿using System;

using AYN.Data.Models;
using AYN.Services.Mapping;

namespace AYN.Web.ViewModels.Administration.Ads
{
    public class GetAllAdsViewModel : IMapFrom<Ad>
    {
        public string Name { get; set; }

        public string AddedByUserUsername { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? ArchivedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsPromoted { get; set; }

        public DateTime? PromotedOn { get; set; }

        public DateTime? PromotedUntil { get; set; }
    }
}
