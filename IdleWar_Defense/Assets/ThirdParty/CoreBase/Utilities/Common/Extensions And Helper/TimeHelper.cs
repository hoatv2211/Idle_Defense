﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Utilities.Common
{
    public static class StringBuilderExtension
    {
        public static StringBuilder Clear(this StringBuilder pBuilder)
        {
            pBuilder.Length = 0;
            pBuilder.Capacity = 0;
            return pBuilder;
        }
    }

    public class TimeHelper
    {
        private static StringBuilder mTimeBuilder = new StringBuilder();
        private static bool mCheckingTime;
        private static DateTime? mStartServerTime;
        private static float mAppTimeWhenGetServerTime;

        /// <summary>
        /// 00:00:00
        /// </summary>
        public static string FormatHHMMss(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                if (showFull || t.Hours > 0)
                {
                    //00:00:00
                    return mTimeBuilder.Clear()
                        .Append(t.Hours.ToString("D2")).Append(":")
                          .Append(t.Minutes.ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                }
                else if (t.Hours == 0)
                {
                    if (t.Minutes > 0)
                    {
                        //00:00
                        return mTimeBuilder.Clear()
                          .Append(t.Minutes.ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                    else
                    {
                        //00
                        return mTimeBuilder.Clear()
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                }
            }
            else if (showFull)
            {
                return "00:00:00";
            }

            return "";
        }

        /// <summary>
        /// 00:00:00
        /// </summary>
        public static string FormatMMss(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                if (showFull || t.Hours > 0)
                {
                    //00:00
                    return mTimeBuilder.Clear()
                          .Append((t.Hours * 60 + t.Minutes).ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                }
                else if (t.Hours == 0)
                {
                    if (t.Minutes > 0)
                    {
                        //00:00
                        return mTimeBuilder.Clear()
                          .Append(t.Minutes.ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                    else
                    {
                        //00
                        return mTimeBuilder.Clear()
                            .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                }
            }
            else if (showFull)
            {
                return "00:00";
            }

            return "";
        }

        /// <summary>
        /// Format to 00:00:00:000
        /// </summary>
        /// <returns></returns>
        public static string FormatHHMMssMs(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                //I keep below code as a result to provide that StringBuilder is much faster than string.format
                //StringBuilder create gabrage lesser than string.Format about 65%

                //if (showFull || t.Hours > 0)
                //{
                //    return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                //        t.Hours,
                //        t.Minutes,
                //        t.Seconds,
                //        t.Milliseconds);
                //}
                //else if (t.Hours == 0)
                //{
                //    if (t.Minutes > 0)
                //    {
                //        return string.Format("{0:D2}:{1:D2}:{2:D3}",
                //        t.Minutes,
                //        t.Seconds,
                //        t.Milliseconds);
                //    }
                //    else
                //    {
                //        return string.Format("{0:D2}:{1:D3}",
                //            t.Seconds,
                //            t.Milliseconds);
                //    }
                //}

                if (showFull || t.Hours > 0)
                {
                    //00:00:00:000
                    return mTimeBuilder.Clear()
                        .Append(t.Hours.ToString("D2")).Append(":")
                          .Append(t.Minutes.ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2")).Append(":")
                              .Append(t.Milliseconds.ToString("D3"))
                        .ToString();
                }
                else if (t.Hours == 0)
                {
                    if (t.Minutes > 0)
                    {
                        //00:00:000
                        return mTimeBuilder.Clear()
                          .Append(t.Minutes.ToString("D2")).Append(":")
                            .Append(t.Seconds.ToString("D2")).Append(":")
                              .Append(t.Milliseconds.ToString("D3"))
                        .ToString();
                    }
                    else
                    {
                        //00:000
                        return mTimeBuilder.Clear()
                            .Append(t.Seconds.ToString("D2")).Append(":")
                              .Append(t.Milliseconds.ToString("D3"))
                        .ToString();
                    }
                }
            }
            else if (showFull)
            {
                return "00:00:00:000";
            }

            return "";
        }

        /// <summary>
        /// day:00:00:00
        /// </summary>
        public static string FormatDayHHMMss(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                if (showFull || t.Days > 0)
                {
                    string day = t.Days > 0 ? (t.Days > 1 ? " days" : " day") : "";
                    //00:00:00:000
                    return mTimeBuilder.Clear()
                        .Append(t.Days > 0 ? (t.Days + day) : "").Append(t.Days > 0 ? " " : "")
                          .Append(t.Hours.ToString("D2")).Append(":")
                            .Append(t.Minutes.ToString("D2")).Append(":")
                              .Append(t.Seconds.ToString("D2"))
                        .ToString();
                }
                else if (t.Days == 0)
                {
                    if (t.Hours > 0)
                    {
                        //00:00:000
                        return mTimeBuilder.Clear()
                          .Append(t.Hours.ToString("D2")).Append(":")
                            .Append(t.Minutes.ToString("D2")).Append(":")
                              .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                    else
                    {
                        //00:000
                        return mTimeBuilder.Clear()
                            .Append(t.Minutes.ToString("D2")).Append(":")
                              .Append(t.Seconds.ToString("D2"))
                        .ToString();
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 6h15m7s
        /// </summary>
        public static string FormatHMs(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                if (showFull || t.Hours > 0)
                {
                    //00h00m00s
                    return mTimeBuilder.Clear()
                        .Append(t.Hours).Append("h")
                          .Append(t.Minutes).Append("m")
                            .Append(t.Seconds).Append("s")
                        .ToString();
                }
                else if (t.Hours == 0)
                {
                    if (t.Minutes > 0)
                    {
                        //00m00s
                        return mTimeBuilder.Clear()
                          .Append(t.Minutes).Append("m")
                            .Append(t.Seconds).Append("s")
                        .ToString();
                    }
                    else
                    {
                        //00s
                        return mTimeBuilder.Clear()
                            .Append(t.Seconds).Append("s")
                        .ToString();
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// 12 hours 1 minute 23 seconds
        /// </summary>
        public static string FormatHMsFull(double seconds, bool showFull)
        {
            if (seconds > 0)
            {
                System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);

                if (showFull || t.Hours > 0)
                {
                    if (t.Seconds > 0)
                    {
                        //Hour Minute Second
                        return mTimeBuilder.Clear()
                            .Append(t.Hours).Append(t.Hours <= 1 ? " Hour " : " Hours ")
                              .Append(t.Minutes > 0 ? t.Minutes.ToString() : "").Append(t.Minutes > 0 ? (t.Minutes == 1 ? " Minute " : " Minutes ") : "")
                                .Append(t.Seconds > 0 ? t.Seconds.ToString() : "").Append(t.Seconds > 0 ? (t.Seconds == 1 ? " Second" : " Seconds") : "")
                            .ToString();
                    }
                }
                else if (t.Hours == 0)
                {
                    if (t.Minutes > 0)
                    {
                        //Minute Second
                        return mTimeBuilder.Clear()
                          .Append(t.Minutes > 0 ? t.Minutes.ToString() : "").Append(t.Minutes == 1 ? " Minute " : " Minutes ")
                            .Append(t.Seconds > 0 ? t.Seconds.ToString() : "").Append(t.Seconds > 0 ? (t.Seconds == 1 ? " Second" : " Seconds") : "")
                        .ToString();
                    }
                    else
                    {
                        //Second
                        if (t.Seconds > 0)
                        {
                            return mTimeBuilder.Clear()
                                .Append(t.Seconds).Append(t.Seconds <= 1 ? " Second" : " Seconds")
                            .ToString();
                        }
                        return "";
                    }
                }
            }

            return "";
        }

        public static double GetSecondsTillMindNight(DateTime pFrom)
        {
            var midNight = System.DateTime.Today.AddDays(1);
            var remainingTime = (midNight - pFrom).TotalSeconds;

            return remainingTime;
        }

        public static DateTime ConvertToLocalTime(DateTime pVietNamTime)
        {
            var utcTime = pVietNamTime.AddHours(-7);
            var zone = TimeZoneInfo.Local.GetUtcOffset(utcTime);
            var time = utcTime + zone;
            return time;
        }

        /// <summary>
        /// Get start of week of date
        /// </summary>
        public static DateTime GetStartTimeOfWeekDay(DateTime pDate, DayOfWeek pDay)
        {
            int dayOfWeek = (int)pDate.DayOfWeek;
            var startTimeOfMonday = pDate.AddDays(-dayOfWeek + 1).Date;
            if (pDay > DayOfWeek.Sunday)
                return startTimeOfMonday.AddDays((int)pDay - 1);
            else
                return startTimeOfMonday.AddDays(6);
        }

        /// <summary>
        /// Get end of week of date
        /// </summary>
        public static DateTime GetEndTimeOfWeekDay(DateTime pDate, DayOfWeek pDay)
        {
            var startTime = GetStartTimeOfWeekDay(pDate, pDay);
            return startTime.AddDays(1).Date;
        }

        /// <summary>
        /// Get start time of month next of date
        /// </summary>
        public static DateTime GetStartTimeOfMonth(DateTime pDate)
        {
            return new DateTime(pDate.Year, pDate.Month, 1).Date;
        }

        /// <summary>
        /// Get end of moneth next of date
        /// </summary>
        public static DateTime GetEndTimeOfMonth(DateTime pDate)
        {
            var firstDayOfMonth = new DateTime(pDate.Year, pDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var lastTimeOfMonth = lastDayOfMonth.AddDays(1).Date;
            return lastTimeOfMonth;
        }

        public static DateTime? GetServerTime()
        {
            if (mAppTimeWhenGetServerTime != 0)
                return mStartServerTime.Value.AddSeconds(Time.unscaledTime - mAppTimeWhenGetServerTime);
            else
                return null;
        }

        public static void CheckServerTime(Action<bool> pOnFinished)
        {
            if (mCheckingTime)
                return;

            string url = "https://showcase.api.linx.twenty57.net/UnixTime/tounix?date=now";

            //var form = new WWWForm();
            UnityWebRequest.ClearCookieCache();
            var request = UnityWebRequest.Get(url);
            request.SendWebRequest();

            mCheckingTime = true;
            WaitUtil.Start(() => request.isDone, () =>
            {
                mCheckingTime = false;
                if (request.isNetworkError)
                {
                    pOnFinished.Raise(false);
                }
                else
                {
                    if (request.responseCode == 200)
                    {
                        var text = request.downloadHandler.text;
                        long unixTime = 0;
                        if (long.TryParse(text, out unixTime))
                            mStartServerTime = Config.UnixTimeStampToDateTime(unixTime);
                        mAppTimeWhenGetServerTime = Time.unscaledTime;
                        pOnFinished.Raise(true);
                    }
                    else
                    {
                        pOnFinished.Raise(false);
                    }
                }
            });
        }


        /// <summary>
        /// Get mod in seconds from amount of time
        /// </summary>
        /// <param name="pIntervalInSeconds"></param>
        /// <param name="pPassedSeconds"></param>
        /// <returns>Step Count and Remain Seconds</returns>
        public static int CalcTimeStepsPassed(long pPassedSeconds, long pIntervalInSeconds, out long pModSeconds)
        {
            int stepPassed = 0;
            pModSeconds = pIntervalInSeconds;
            if (pPassedSeconds > 0)
            {
                stepPassed += Mathf.FloorToInt(pPassedSeconds / pIntervalInSeconds);
                pModSeconds = pPassedSeconds % pIntervalInSeconds;
            }
            return stepPassed;
        }

        /// <summary>
        /// Server time have format type MM/dd/yyyy HH:mm:ss
        /// </summary>
        public static bool TryParse(string pServerTime, out DateTime pTime)
        {
            string[] formats =
            {
                "MM/dd/yyyy hh:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy hh:mm:ss tt",
                "MM/dd/yyyy hh:mm:ss tt zzz",
                "MM/dd/yyyy hh:mm",
                "MM/dd/yyyy HH:mm",
            };

            var enUS = new System.Globalization.CultureInfo("en-US");
            pTime = DateTime.MinValue;
            foreach (var format in formats)
                if (DateTime.TryParseExact(pServerTime, format, enUS, System.Globalization.DateTimeStyles.None, out pTime))
                    return true;
            return false;
        }

        public static double GetSecondsTillDayOfWeek(DayOfWeek pDayOfWeek, DateTime pNow)
        {
            int dayCount = 0;
            switch (pDayOfWeek)
            {
                case DayOfWeek.Sunday:
                    dayCount = (int)DayOfWeek.Sunday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Monday:
                    dayCount = (int)DayOfWeek.Monday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Tuesday:
                    dayCount = (int)DayOfWeek.Tuesday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Wednesday:
                    dayCount = (int)DayOfWeek.Wednesday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Thursday:
                    dayCount = (int)DayOfWeek.Thursday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Friday:
                    dayCount = (int)DayOfWeek.Friday - (int)pNow.DayOfWeek;
                    break;
                case DayOfWeek.Saturday:
                    dayCount = (int)DayOfWeek.Saturday - (int)pNow.DayOfWeek;
                    break;
            }
            if (dayCount <= 0)
                dayCount += 7;
            double seconds = (pNow.AddDays(dayCount).Date - pNow).TotalSeconds;
            return seconds;
        }

        public static double GetSecondsTillEndDayOfWeek(DayOfWeek pDayOfWeek, DateTime pNow)
        {
            int dayCount = 0;
            switch (pDayOfWeek)
            {
                case DayOfWeek.Sunday:
                    dayCount = (int)DayOfWeek.Sunday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Monday:
                    dayCount = (int)DayOfWeek.Monday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Tuesday:
                    dayCount = (int)DayOfWeek.Tuesday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Wednesday:
                    dayCount = (int)DayOfWeek.Wednesday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Thursday:
                    dayCount = (int)DayOfWeek.Thursday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Friday:
                    dayCount = (int)DayOfWeek.Friday - (int)pNow.DayOfWeek + 1;
                    break;
                case DayOfWeek.Saturday:
                    dayCount = (int)DayOfWeek.Saturday - (int)pNow.DayOfWeek + 1;
                    break;
            }
            if (dayCount <= 0)
                dayCount += 7;
            double seconds = (pNow.AddDays(dayCount).Date - pNow).TotalSeconds;
            return seconds;
        }
    }

    public static class TimeExtension
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ConvertToTimestamp(this DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}