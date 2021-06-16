using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba_Desarrollo.Model
{
    public class RequestModel
    {
        public string departure_airport;
        public string arrival_airport;
        public string departure_time;
        public string arrival_time;
        public string airline;
        public string flight_number;
        public decimal? price;

        public string Departure_airport
        {
            get { return departure_airport; }   
            set { departure_airport = value; }  
        }

        public string Arrival_airport
        {
            get { return arrival_airport; }
            set { arrival_airport = value; }
        }

        public string Departure_time
        {
            get { return departure_time; }
            set { departure_time = value; }
        }

        public string Arrival_time
        {
            get { return arrival_time; }
            set { arrival_time = value; }
        }

        public string Airline
        {
            get { return airline; }
            set { airline = value; }
        }

        public string Flight_number
        {
            get { return flight_number; }
            set { flight_number = value; }
        }

        public decimal? Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
