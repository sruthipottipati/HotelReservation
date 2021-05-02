using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HotelReservationConsole
{
    class Program
    {
        string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Sruthi\HotelReservation\HotelReservation\HotelRoomDetails.mdf;Integrated Security=True;Connect Timeout=30";
        public int StartDate;
        public int EndDate;
        //Hoteldetails queried from database
        List<Hoteldetails> list = new List<Hoteldetails>();
        //Roomdetails queried from database
        List<int> roomNo = new List<int>();
        int availableRoomNo = 0;

        static void Main(string[] args)
        {
            Program obj = new Program();
            obj.GetRoomDetails();
            obj.GeHotelReservationDetails();
            if (obj.CheckAvailability())
            {
                if (obj.availableRoomNo > 0)
                {
                    Console.WriteLine("Room number " + obj.availableRoomNo + " is available. Please confirm to reserve the room (Y/N)");
                    string confirm = Console.ReadLine();
                    if (confirm.ToString() == "Y" || confirm.ToString()=="y")
                    {
                        int rowsAffected=obj.insertHotelReservation();
                        if (rowsAffected > 0)
                            Console.WriteLine("Booking Confirmed");
                    }
                }
            }

        }
        // Method Name : GetRoomDetails
        // Method Description : query the database to get the number of rooms available in a hotel
        public void GetRoomDetails()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"select * from RoomDetails";
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            roomNo.Add(Convert.ToInt32(reader["RoomNo"]));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rooms available.");
                    }
                    reader.Close();

                }
            }
        }

        //Method Name : GetHotelReservationDetails
        //Method Description : query the database to get the rooms details which are booked in a hotel
        public void GeHotelReservationDetails()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"select * from HotelReservation";
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(
                                new Hoteldetails
                                {
                                    RoomNo = Convert.ToInt32(reader["RoomNo"]),
                                    StartDate = Convert.ToInt32(reader["StartDate"]),
                                    EndDate = Convert.ToInt32( reader["EndDate"])
                                }

                            );

                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();

                }
            }
        }

        //Method Name : CheckAvailability
        //Method Description : To check whether rooom is available or not based on user input start date and end date
        public string ValidateUserInput(string strStartDate, string strEndDate)
        {
            string message = string.Empty;
            if((strStartDate == string.Empty)||(strEndDate == string.Empty))
            {

                message = "StartDate / End Date is empty";
            }
                       
            else if ((Convert.ToInt32(strStartDate) < 0) || (Convert.ToInt32(strEndDate) < 0))
            {
                message="Start Date/End Date should be greater than 0.";
          
            }
            else if (Convert.ToInt32(strStartDate) > Convert.ToInt32(strEndDate))
            {
                message = "Please enter valid start date. End date should be greater than Start date";
            }
            return message;

        }
        public bool CheckAvailability()
        {
            bool messageChk = true;
            string message = string.Empty;
            string strStartDate = string.Empty;
            string strEndDate= string.Empty;
  
            //check null exception for start date
            while (messageChk)
            {
                Console.WriteLine("Please enter the StartDate of Room Booking . Start date should be between 0 to 365");
                strStartDate = Console.ReadLine();
                Console.WriteLine("Please enter the EndDate of Room Booking. End Date should be between 0 to 365");
                strEndDate = Console.ReadLine();

                message = ValidateUserInput(strStartDate, strEndDate);

                if (message == string.Empty)
                {
                    messageChk = false;             

                }
                else
                {
                    Console.WriteLine("Error Message: " + message);
                    Console.WriteLine();
                    messageChk = true;
                }
            }

            if (message == string.Empty)
            {
                StartDate = Convert.ToInt32(strStartDate);
                EndDate = Convert.ToInt32(strEndDate);
                List<Hoteldetails> rowList = new List<Hoteldetails>();

                for (int i = 0; i < roomNo.Count - 1; i++)
                {
                    rowList = (from x in list where ((x.RoomNo == roomNo[i]) && (x.StartDate <= StartDate  && x.EndDate >= StartDate)) select x).ToList();
                    if (rowList.Count == 0)
                    {
                        availableRoomNo = roomNo[i];
                        return true;
                    }

                }
                if (rowList.Count == 0)
                {
                    return true;
                }
                else
                    return false;
            }
            
            return false;
          

        }


        //Method Name: InsertHotelREservation
        //Method Description : To insert the new user hotel booking details if rooms are available 
        public int insertHotelReservation()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = @"INSERT INTO HotelReservation(RoomNo,StartDate, EndDate) VALUES('" + availableRoomNo + "', '" + StartDate + "', '" + EndDate + "')";
                using (var command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }

    //Class object for hotel details.
    public class Hoteldetails
    {

        public int StartDate { get ; set ; }
        public int RoomNo { get; set; }
        public int EndDate { get; set; }
    }

   
   
}
