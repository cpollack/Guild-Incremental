using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameTime
{
    public GameTime(int day = 0, float hour = 0f)
    {
        this.day = day;
        this.hour = hour;
    }

    public GameTime(GameTime time)
    {
        day = time.day;
        hour = time.hour;
    }

    public int day;
    public float hour;

    public void Set(int days, float hours)
    {
        day = days;
        hour = hours;
    }

    public void Set(GameTime time)
    {
        day = time.day;
        hour = time.hour;
    }

    public void SetByHours(float hours)
    {
        if (hours >= 24)
        {
            day = (int)hours / 24;
            hours -= day * 24;
        }
        hour = hours;
    }

    public void AddHours(float hours)
    {
        int incDay = (int)hours / 24;
        day += incDay;
        hour += hours - (incDay * 24);
        if (hour >= 24)
        {
            day++;
            hour -= 24;
        }
    }

    public void SubtractHours(float hours)
    {
        int subDay = (int)hours / 24;
        day -= subDay;
        if (day < 0)
        {
            day = 0;
            hours = 0;
            return;
        }
        hour -= hours - (subDay * 24);
        if (hours < 0)
        {            
            day--;
            if (day < 0)
            {
                day = 0;
                hours = 0;
            }
            else hours += 24;
        }
    }

    public GameTime GetDifference(GameTime b)
    {
        GameTime dif = new GameTime();
        dif.day = day - b.day;
        dif.hour = hour - b.hour;
        return dif;
    }

    public float GetHours()
    {
        return (day * 24) + hour;
    }

    public static bool operator >(GameTime a, GameTime b)
    {
        if (a.day > b.day) return true;
        if (a.day < b.day) return false;
        if (a.hour > b.hour) return true;
        return false;
    }

    public static bool operator <(GameTime a, GameTime b)
    {
        if (a.day < b.day) return true;
        if (a.day > b.day) return false;
        if (a.hour < b.hour) return true;
        return false;
    }

    public static bool operator >=(GameTime a, GameTime b)
    {
        if (a.day > b.day) return true;
        if (a.day < b.day) return false;
        if (a.hour >= b.hour) return true;
        return false;
    }

    public static bool operator <=(GameTime a, GameTime b)
    {
        if (a.day < b.day) return true;
        if (a.day > b.day) return false;
        if (a.hour <= b.hour) return true;
        return false;
    }

    public override string ToString()
    {
        string strTime = "";
        if (day > 0)
        {
            strTime = day + " Day" + (day > 1 ? "s" : "");
        }
        if (strTime.Length > 0) strTime += " ";
        strTime += ((int)hour).ToString("0") + " Hour" + ((int)hour == 1 ? "" : "s");
        return strTime;
    }

    public string GetFormattedTime(bool showHours = false)
    {
        int clock12 = Mathf.FloorToInt(hour) % 12;
        if (clock12 == 0) clock12 = 12;
        return "Day " + day.ToString() + (showHours ? " " + clock12.ToString() + (Mathf.Floor(hour) <= 11 ? "am" : "pm") : "");
    }
}
