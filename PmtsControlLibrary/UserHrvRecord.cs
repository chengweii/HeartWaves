using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PmtsControlLibrary
{
    public class UserHrvRecord
    {
        public UserHrvRecord()
        {

        }
        public ArrayList HRVData = new ArrayList();
        public ArrayList EPData = new ArrayList();
        public ArrayList AvailData = new ArrayList();
        public ArrayList MarkData = new ArrayList();
        public ArrayList IBIData = new ArrayList();
        public ArrayList PWRData = new ArrayList();
        public ArrayList TimeData = new ArrayList();
        public ArrayList FreData = new ArrayList();
        public int Level = 0;
        public double Pressure = 0;
        public double Adjust = 0;
        public double Stable = 0;
        public double Score = 0;
        public double AnsBalance = 0;
        public double HrvScore = 0;
        public double TimeLength = 0;
        public DateTime StartTime;
        public DateTime EndTime;
        public int RecordType;
        public int Mood;
        public string Report;
        public string Id;
    }

}
