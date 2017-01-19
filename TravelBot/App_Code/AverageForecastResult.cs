using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelBot.App_Code
{
    public class FeaturesPlanner
    {
        public int planner { get; set; }
    }

    public class ResponsePlanner
    {
        public string version { get; set; }
        public string termsofService { get; set; }
        public FeaturesPlanner features { get; set; }
    }

    public class DateStart
    {
        public Date date { get; set; }
    }

    public class Date2
    {
        public string epoch { get; set; }
        public string pretty { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int yday { get; set; }
        public int hour { get; set; }
        public string min { get; set; }
        public int sec { get; set; }
        public string isdst { get; set; }
        public string monthname { get; set; }
        public string monthname_short { get; set; }
        public string weekday_short { get; set; }
        public string weekday { get; set; }
        public string ampm { get; set; }
        public string tz_short { get; set; }
        public string tz_long { get; set; }
    }

    public class DateEnd
    {
        public Date2 date { get; set; }
    }

    public class PeriodOfRecord
    {
        public DateStart date_start { get; set; }
        public DateEnd date_end { get; set; }
    }

    public class Min
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Avg
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Max
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class TempHigh
    {
        public Min min { get; set; }
        public Avg avg { get; set; }
        public Max max { get; set; }
    }

    public class Min2
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Avg2
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Max2
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class TempLow
    {
        public Min2 min { get; set; }
        public Avg2 avg { get; set; }
        public Max2 max { get; set; }
    }

    public class Min3
    {
        public string @in { get; set; }
        public string cm { get; set; }
    }

    public class Avg3
    {
        public string @in { get; set; }
        public string cm { get; set; }
    }

    public class Max3
    {
        public string @in { get; set; }
        public string cm { get; set; }
    }

    public class Precip
    {
        public Min3 min { get; set; }
        public Avg3 avg { get; set; }
        public Max3 max { get; set; }
    }

    public class Min4
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Avg4
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Max4
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class DewpointHigh
    {
        public Min4 min { get; set; }
        public Avg4 avg { get; set; }
        public Max4 max { get; set; }
    }

    public class Min5
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Avg5
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class Max5
    {
        public string F { get; set; }
        public string C { get; set; }
    }

    public class DewpointLow
    {
        public Min5 min { get; set; }
        public Avg5 avg { get; set; }
        public Max5 max { get; set; }
    }

    public class CloudCover
    {
        public string cond { get; set; }
    }

    public class Tempoversixty
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofsunnycloudyday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofprecip
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofrainday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofpartlycloudyday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofcloudyday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Tempoverfreezing
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceoffogday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofsnowonground
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceoftornadoday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofwindyday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofsultryday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofhumidday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Tempbelowfreezing
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Tempoverninety
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofthunderday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofhailday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class Chanceofsnowday
    {
        public string name { get; set; }
        public string description { get; set; }
        public string percentage { get; set; }
    }

    public class ChanceOf
    {
        public Tempoversixty tempoversixty { get; set; }
        public Chanceofsunnycloudyday chanceofsunnycloudyday { get; set; }
        public Chanceofprecip chanceofprecip { get; set; }
        public Chanceofrainday chanceofrainday { get; set; }
        public Chanceofpartlycloudyday chanceofpartlycloudyday { get; set; }
        public Chanceofcloudyday chanceofcloudyday { get; set; }
        public Tempoverfreezing tempoverfreezing { get; set; }
        public Chanceoffogday chanceoffogday { get; set; }
        public Chanceofsnowonground chanceofsnowonground { get; set; }
        public Chanceoftornadoday chanceoftornadoday { get; set; }
        public Chanceofwindyday chanceofwindyday { get; set; }
        public Chanceofsultryday chanceofsultryday { get; set; }
        public Chanceofhumidday chanceofhumidday { get; set; }
        public Tempbelowfreezing tempbelowfreezing { get; set; }
        public Tempoverninety tempoverninety { get; set; }
        public Chanceofthunderday chanceofthunderday { get; set; }
        public Chanceofhailday chanceofhailday { get; set; }
        public Chanceofsnowday chanceofsnowday { get; set; }
    }

    public class Trip
    {
        public string title { get; set; }
        public string airport_code { get; set; }
        public string error { get; set; }
        public PeriodOfRecord period_of_record { get; set; }
        public TempHigh temp_high { get; set; }
        public TempLow temp_low { get; set; }
        public Precip precip { get; set; }
        public DewpointHigh dewpoint_high { get; set; }
        public DewpointLow dewpoint_low { get; set; }
        public CloudCover cloud_cover { get; set; }
        public ChanceOf chance_of { get; set; }
    }

    public class AverageForecastResult
    {
        public ResponsePlanner response { get; set; }
        public Trip trip { get; set; }
    }
}