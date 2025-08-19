using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Models.PayMob
{
    public class PaymobSettings
    {
        public string ApiKey { get; set; }
        public int IntegrationId { get; set; }
        public int IframeId { get; set; }
        public string PaymentBaseUrl { get; set; }
    }

}
