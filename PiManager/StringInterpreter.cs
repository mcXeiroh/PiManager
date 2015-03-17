using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiManager
{
    class StringInterpreter
    {
        /**
         * Builds a message which can be send to the networkstream
         * 
         * @param Create the message out of the pin.
         * @return Builded message
         */
        public static string BuildMessage(Pin pin)
        {
            return pin.IO + " " + pin.No + " " + pin.State + "\n";
        }

        /**
         * Converts a message to a pin
         * 
         * @param Message to convert
         * @return Converted pin
         */
        public static Pin ReadMessage(string msg)
        {
            Pin readPin = new Pin();

            char[] msgAry = msg.ToCharArray();

            if (msgAry[0].Equals('i'))
                readPin.IO = Pin.GPIO.i;
            else
                readPin.IO = Pin.GPIO.o;

            readPin.No = Convert.ToInt32(msgAry[2]);

            if (msgAry[4].Equals('a')) //Activate
                readPin.State = true;
            else
                readPin.State = false;

            return readPin;
        }
    }

    
}
