using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MySqlConnector;
using Prueba_Desarrollo.Model;
using Microsoft.Extensions.Configuration;
using System.Xml;

namespace Prueba_Desarrollo.Controllers
{
    public class BookingsController : Controller
    {
        //Servicio desarrollado para prueba tecnica con Netactica
        //Se Agregó MySqlConnector para gestionar con MySql una BD para almacenar las reservaciones.

        //Servicio para almacenar reservas nuevas
        [HttpPost]
        [Route("/Bookingservice/CreateBookings")]
        public virtual IActionResult CreateBookingsService([FromBody] RequestModel request)
        {
            //Validación de campos obligatorios
            bool flag = false;
            if (request.Departure_airport != null)
            {
                if (request.Departure_airport.Trim() == "")
                    flag = true;
            }
            else 
            {
                flag = true;
            }
            if (request.Arrival_airport != null)
            {
                if (request.Arrival_airport.Trim() == "")
                    flag = true;
            }
            else
            {
                flag = true;
            }
            if (request.Departure_time != null)
            {
                if (request.Departure_time.Trim() == "")
                    flag = true;
            }
            else
            {
                flag = true;
            }
            if (request.Arrival_time != null)
            {
                if (request.Arrival_time.Trim() == "")
                    flag = true;
            }
            else
            {
                flag = true;
            }
            if (request.Airline != null)
            {
                if (request.Airline.Trim() == "")
                    flag = true;
            }
            else
            {
                flag = true;
            }
            if (request.Flight_number != null)
            {
                if (request.Flight_number.Trim() == "")
                    flag = true;
            }
            else
            {
                flag = true;
            }
            if (request.Price != null)
            {
                if (request.price <0)
                    flag = true;
            }
            else
            {
                flag = true;
            }

            if (flag == true) 
            {
                return BadRequest("Error: Faltan campos obligatorios");
            }

            //Almacenar información de la resevar
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            string conexion = configuration["ConnectionStrings:Default"];

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Querys.xml");

            string selectCommandText = ReaderXml(path, "CREATE_BOOKING");
            selectCommandText = selectCommandText.Replace("@v1", request.Departure_airport);
            selectCommandText = selectCommandText.Replace("@v2", request.Arrival_airport);
            selectCommandText = selectCommandText.Replace("@v3", request.Departure_time);
            selectCommandText = selectCommandText.Replace("@v4", request.Arrival_time);
            selectCommandText = selectCommandText.Replace("@v5", request.Airline);
            selectCommandText = selectCommandText.Replace("@v6", request.Flight_number);
            selectCommandText = selectCommandText.Replace("@v7", request.Price.ToString());

            string Id_Booking = "588";

            try
            { 
                using (MySqlConnection selectConnection = new MySqlConnection(conexion))
                {
                    selectConnection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(selectCommandText))
                    {
                        using (cmd.Connection = selectConnection)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                   Id_Booking = reader.GetString(0);
                                }
                            }
                        
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                return BadRequest("Error: Fallo de conexión - "+Ex);
            }

            return new ObjectResult("ID:"+Id_Booking);
        }

        //Servicio para recuperar reservación por id
        [HttpGet]
        [Route("/Bookingservice/RecoveryBookings")]
        public virtual IActionResult RecoveryBookingsService([FromQuery] int id)
        {
            //Buscar en BD reservación por Id
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            string conexion = configuration["ConnectionStrings:Default"];

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Querys.xml");

            string selectCommandText = ReaderXml(path, "SEARCH_BOOKING");
            selectCommandText = selectCommandText.Replace("@v1","WHERE ID = "+ id.ToString());
            RequestModel Booking = new RequestModel();

            try
            {
                using (MySqlConnection selectConnection = new MySqlConnection(conexion))
                {
                    selectConnection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(selectCommandText))
                    {
                        using (cmd.Connection = selectConnection)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Booking.Departure_airport = reader.GetString(0);
                                    Booking.Arrival_airport = reader.GetString(1);
                                    Booking.Departure_time = reader.GetString(2);
                                    Booking.Arrival_time = reader.GetString(3);
                                    Booking.Airline = reader.GetString(4);
                                    Booking.Flight_number = reader.GetString(5);
                                    Booking.Price = reader.GetDecimal(6);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                return BadRequest("Error: Fallo de conexión - " + Ex);
            }

            return Json(Booking);
        }

        //Servicio para listar reservaciones por criterios seleccioandos
        [HttpPost]
        [Route("/Bookingservice/ListBookings")]
        public virtual IActionResult ListBookingsService([FromBody] RequestModel request)
        {
            //Validar filtros seleccionados
            string obj = "";
            bool flag = false;
            if (request.Departure_airport != null)
            {
                if (request.Departure_airport.Trim() != "")
                {
                    if (flag == false) 
                    {
                        obj = obj + " DP_AIRPORT = '" + request.Departure_airport + "'";
                        flag = true;
                    }
                    else
                    { 
                        obj = obj + " AND DP_AIRPORT = '" + request.Departure_airport;
                    }
                }
            }

            if (request.Arrival_airport != null)
            {
                if (request.Arrival_airport.Trim() != "")
                {
                    if (flag == false)
                    {
                        obj = obj + " ARR_AIRPORT = '" + request.Arrival_airport + "'";
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND ARR_AIRPORT = '" + request.Arrival_airport + "'";
                    }
                }
            }

            if (request.Departure_time != null)
            {
                if (request.Departure_time.Trim() != "")
                {
                    if (flag == false)
                    {
                        obj = obj + " DP_TIME = '" + request.Departure_time +"'";
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND DP_TIME = '" + request.Departure_time + "'";
                    }
                }
            }

            if (request.Arrival_time != null)
            {
                if (request.Arrival_time.Trim() != "")
                {
                    if (flag == false)
                    {
                        obj = obj + " ARR_TIME = '" + request.Arrival_time + "'";
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND ARR_TIME = '" + request.Arrival_time + "'";
                    }
                }
            }

            if (request.Airline != null)
            {
                if (request.Airline.Trim() != "")
                {
                    if (flag == false)
                    {
                        obj = obj + " AIRLINE = '" + request.Airline + "'";
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND AIRLINE = '" + request.Airline + "'";
                    }
                }
            }

            if (request.Flight_number != null)
            {
                if (request.Flight_number.Trim() != "")
                {
                    if (flag == false)
                    {
                        obj = obj + " FLIGHT_NUMB = '" + request.Flight_number + "'";
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND FLIGHT_NUMB = '" + request.Flight_number + "'";
                    }
                }
            }

            if (request.Price != null)
            {
                if (request.Price < 0)
                {
                    if (flag == false)
                    {
                        obj = obj + " PRICE = " + request.Price.ToString();
                        flag = true;
                    }
                    else
                    {
                        obj = obj + " AND PRICE = " + request.Price.ToString();
                    }
                }
            }




            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            string conexion = configuration["ConnectionStrings:Default"];

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Querys.xml");

            string selectCommandText = ReaderXml(path, "SEARCH_BOOKING");
            selectCommandText = selectCommandText.Replace("@v1", "WHERE "+obj);
            List<RequestModel> List = new List<RequestModel>();

            try
            {
                using (MySqlConnection selectConnection = new MySqlConnection(conexion))
                {
                    selectConnection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(selectCommandText))
                    {
                        using (cmd.Connection = selectConnection)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    RequestModel Booking = new RequestModel();
                                    Booking.Departure_airport = reader.GetString(0);
                                    Booking.Arrival_airport = reader.GetString(1);
                                    Booking.Departure_time = reader.GetString(2);
                                    Booking.Arrival_time = reader.GetString(3);
                                    Booking.Airline = reader.GetString(4);
                                    Booking.Flight_number = reader.GetString(5);
                                    Booking.Price = reader.GetDecimal(6);
                                    List.Add(Booking);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                return BadRequest("Error: Fallo de conexión - " + Ex);
            }

            return Json(List);
        }

        //Buscar Querys dentro de Archivo XML
        public string ReaderXml(string path, string query) 
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            string read_query = "";
            XmlNodeList xQuery = xDoc.GetElementsByTagName("QUERY");
            XmlNodeList xLista = ((XmlElement)xQuery[0]).GetElementsByTagName(query);
            foreach (XmlElement nodo in xLista)
            {
                read_query = nodo.InnerText;
            }
            return read_query;
        }
    }
}