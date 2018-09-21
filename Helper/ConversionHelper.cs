﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Helper
{
    public class ConversionHelper
    {
        public DateTime julianTOCalender(string julianDate, string julianCC)
        {
            DateTime calenderDate = new DateTime();
            try
            {
                int jDate = Convert.ToInt32(julianDate);
                int day = jDate % 1000;
                int year = (jDate - day + Convert.ToInt32(julianCC + "00000")) / 1000;
                var date1 = new DateTime(year, 1, 1);
                calenderDate = date1.AddDays(day - 1);
            }
            catch (Exception)
            {

            }
            return calenderDate;
        }
        public static double? ToJulianDate(DateTime date)
        {
            try
            {
                return date.ToOADate() + 2415018.5;
            }
            catch (Exception)
            {

            }
            return null;
        }
        public static int convertSelectedOption(string selectedOption)
        {
            try
            {
                selectedOption = selectedOption.ToLower();
                if (selectedOption == "a")
                {
                    return 1;
                }
                else if (selectedOption == "b")
                {
                    return 2;
                }
                else if (selectedOption == "c")
                {
                    return 3;
                }
                else if (selectedOption == "d")
                {
                    return 4;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }
        public static string dateFormat(string myStr)
        {
            try
            {
                if (myStr.Trim().Contains("/"))
                {
                    string[] ssizeBySlash = myStr.Split('/');
                     myStr = string.Format("{0}/{1}/{2}", ssizeBySlash[0], ssizeBySlash[1], ssizeBySlash[2]);

                }
                if (myStr.Trim().Contains("-"))
                {
                    string[] ssizeByDash = myStr.Split('-');
                     myStr = string.Format("{0}/{1}/{2}", ssizeByDash[0], ssizeByDash[1], ssizeByDash[2]);

                }
                if (myStr.Trim().Contains(" "))
                {
                    string[] ssize = myStr.Split(new char[0]);
                     myStr = string.Format("{0}/{1}/{2}", ssize[0], ssize[1], ssize[2]);

                }
            }
            catch (Exception ex)
            {

            }
            return myStr;
        }

    }
}