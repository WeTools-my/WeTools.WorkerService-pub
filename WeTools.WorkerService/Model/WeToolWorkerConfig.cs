using System;

namespace WeTools.WorkerService.Model
{
    public class WeToolWorkerConfig
    {
        public string Name { get; set; }
        public string WorkerName { get; set; }
        public bool Enable { get; set; }

        private int _delay;
        public int Delay { 
            get {
                   return _delay * 1000;
             }
            set
            {
                if (value <=0)
                {
                    throw new ArgumentException("无效delay值，delay必须大于0");
                }
                _delay = value;
            }
        }


        //private string _cron;
        //public string Cron { 
        //    get {
        //        return _cron;
        //    } 
        //    set {

        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            _cron = value;

        //            CronExpression expression = CronExpression.Parse(_cron, CronFormat.IncludeSeconds);

        //            DateTime? next = expression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);

        //            DateTime? next2 = expression.GetNextOccurrence(next.Value, TimeZoneInfo.Local);

        //            Delay = next2.Value - next.Value;

        //        }
                

        //    }
        //}

    }
}
