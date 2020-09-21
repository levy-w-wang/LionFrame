﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LionFrame.Domain.BaseDomain
{
    public abstract class BaseCommonModel : BaseModel
    {
        protected BaseCommonModel()
        {
            Deleted = false;
            CreatedTime = DateTime.Now;
            UpdatedTime = DateTime.Now;
        }

        [Column(Order = 90)]
        public bool Deleted { get; set; }

        [Column(Order = 91), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedTime { get; set; }

        [Column(Order = 92)]
        public long CreatedBy { get; set; }

        [Column(Order = 93),DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedTime { get; set; }

        [Column(Order = 94)]
        public long UpdatedBy { get; set; }
    }
}