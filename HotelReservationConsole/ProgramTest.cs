using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace HotelReservationConsole
{
    [TestFixture]
   public  class ProgramTest
    {
        Program pgm = new Program();
        // Test case to check availability of rooms if startdate>end date
        [Test]
        public void ValidateUserInput()
        {
            string startDate = string.Empty;
            string endDate = string.Empty;
            string message = string.Empty;
            message = pgm.ValidateUserInput(startDate,endDate);
            Assert.AreEqual(message, "StartDate / End Date is empty");
            startDate = "-1";
            endDate = "0";
            message = pgm.ValidateUserInput(startDate, endDate);
            Assert.AreEqual(message, "Start Date/End Date should be greater than 0.");
            startDate = "3";
            endDate = "1";
            message = pgm.ValidateUserInput(startDate, endDate);
            Assert.AreEqual(message, "Please enter valid start date. End date should be greater than Start date")
;
        }
    }
}
