﻿using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Alkhaligya.BLL.Dtos.Auth
{
    public class GeneralRespnose
    {
        public bool successed { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
