﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TopCourse.Core.Utilities.Paging
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage => (int)Math.Ceiling((decimal)TotalItems / ItemPerPage);
        public string UrlParam { get; set; }
    }
}
