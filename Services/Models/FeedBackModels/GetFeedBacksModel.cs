using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.FeedBackModels
{
    public class GetFeedBacksModel
    {
        public Guid Id { get; set; }
        public Guid ProductID { get; set; }
        public Guid AccountID { get; set; }
        public string AccountName { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
    }
}
