namespace _2024_WpfApp7
{
    public class Extras
    {
        public string api_key { get; set; }
    }

    public class Field
    {
        public string id { get; set; }
        public string type { get; set; }
        public Info info { get; set; }
    }

    public class Info
    {
        public string label { get; set; }
    }

    public class Links
    {
        public string start { get; set; }
        public string next { get; set; }
    }

    public class Record
    {
        public string sitename { get; set; }
        public string county { get; set; }
        public string aqi { get; set; }
        public string pollutant { get; set; }
        public string status { get; set; }
        public string so2 { get; set; }
        public string co { get; set; }
        public string o3 { get; set; }
        public string o3_8hr { get; set; }
        public string pm10 { get; set; }

        public string pm25 { get; set; }
        public string no2 { get; set; }
        public string nox { get; set; }
        public string no { get; set; }
        public string wind_speed { get; set; }
        public string wind_direc { get; set; }
        public string publishtime { get; set; }
        public string co_8hr { get; set; }

        public string pm25_avg { get; set; }
        public string pm10_avg { get; set; }
        public string so2_avg { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string siteid { get; set; }
    }

    public class AQIData
    {
        public List<Field> fields { get; set; }
        public string resource_id { get; set; }
        public Extras __extras { get; set; }
        public bool include_total { get; set; }
        public string total { get; set; }
        public string resource_format { get; set; }
        public string limit { get; set; }
        public string offset { get; set; }
        public Links _links { get; set; }
        public List<Record> records { get; set; }
    }
}
