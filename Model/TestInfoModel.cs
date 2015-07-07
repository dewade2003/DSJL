using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace DSJL.Model
{
    public class TestInfoModel : INotifyPropertyChanged,ICloneable
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Index
        {
            get;
            set;
        }

        #region Athlete Model
        private int _athid;
        private string _ath_code;
        private string _ath_name;
        private string _ath_pinyin;
        private string _ath_sex;
        private DateTime _ath_birthday;
        private string _ath_height;
        private string _ath_weight;
        private string _ath_project;
        private string _ath_mainproject;
        private string _ath_trainyears;
        private string _ath_level;
        private string _ath_team;
        private DateTime _ath_testdate;
        private string _ath_testaddress;
        private string _ath_testmachine;
        private string _ath_teststate;
        private string _ath_remark;
        private int _ath_testid;

        private bool _isChecked = false;

        public bool IsChecked
        {
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
            get { return _isChecked; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int AthID
        {
            set { _athid = value; }
            get { return _athid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Code
        {
            set { _ath_code = value; }
            get { return _ath_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Name
        {
            set { _ath_name = value; }
            get { return _ath_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_PinYin
        {
            set { _ath_pinyin = value; }
            get { return _ath_pinyin; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Sex
        {
            set { _ath_sex = value; }
            get { return _ath_sex; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Ath_Birthday
        {
            set { _ath_birthday = value; }
            get { return _ath_birthday; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Height
        {
            set { _ath_height = value; }
            get { return _ath_height; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Weight
        {
            set { _ath_weight = value; }
            get { return _ath_weight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Project
        {
            set { _ath_project = value; }
            get { return _ath_project; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_MainProject
        {
            set { _ath_mainproject = value; }
            get { return _ath_mainproject; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_TrainYears
        {
            set { _ath_trainyears = value; }
            get { return _ath_trainyears; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Level
        {
            set { _ath_level = value; }
            get { return _ath_level; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Team
        {
            set { _ath_team = value; }
            get { return _ath_team; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Ath_TestDate
        {
            set { _ath_testdate = value; }
            get { return _ath_testdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_TestAddress
        {
            set { _ath_testaddress = value; }
            get { return _ath_testaddress; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_TestMachine
        {
            set { _ath_testmachine = value; }
            get { return _ath_testmachine; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_TestState
        {
            set { _ath_teststate = value; }
            get { return _ath_teststate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Ath_Remark
        {
            set { _ath_remark = value; }
            get { return _ath_remark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Ath_TestID
        {
            set { _ath_testid = value; }
            get { return _ath_testid; }
        }
        #endregion Model

        #region TestInfoModel
        private int _testid;
        private int _ath_id;
        private DateTime _testdate;
        private DateTime _testtime;
        private string _joint_side;
        private string _test_mode;
        private string _joint;
        private string _plane;
        private string _motion_start;
        private string _motion_end;
        private string _speed1;
        private string _speed2;
        private string _acceleration1;
        private string _acceleration2;
        private string _break;
        private string _noofsets;
        private string _noofrepetitions;
        private string _insuredside;
        private string _gravitycomp;
        private string _therapist;
        private string _basefilename;
        private string _datafilename;
        private string _remark;
        private string _actionID;
        /// <summary>
        /// 
        /// </summary>
        public int TestID
        {
            set { _testid = value; }
            get { return _testid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Ath_ID
        {
            set { _ath_id = value; }
            get { return _ath_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime TestDate
        {
            set { _testdate = value; }
            get { return _testdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime TestTime
        {
            set { _testtime = value; }
            get { return _testtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Joint_Side
        {
            set { _joint_side = value; }
            get { return _joint_side; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Test_Mode
        {
            set { _test_mode = value; }
            get { return _test_mode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Joint
        {
            set { _joint = value; }
            get { return _joint; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Plane
        {
            set { _plane = value; }
            get { return _plane; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Motion_Start
        {
            set { _motion_start = value; }
            get { return _motion_start; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Motion_End
        {
            set { _motion_end = value; }
            get { return _motion_end; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Speed1
        {
            set { _speed1 = value; }
            get { return _speed1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Speed2
        {
            set { _speed2 = value; }
            get { return _speed2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Acceleration1
        {
            set { _acceleration1 = value; }
            get { return _acceleration1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Acceleration2
        {
            set { _acceleration2 = value; }
            get { return _acceleration2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Break
        {
            set { _break = value; }
            get { return _break; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string NOOfSets
        {
            set { _noofsets = value; }
            get { return _noofsets; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string NOOfRepetitions
        {
            set { _noofrepetitions = value; }
            get { return _noofrepetitions; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string InsuredSide
        {
            set { _insuredside = value; }
            get { return _insuredside; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Gravitycomp
        {
            set { _gravitycomp = value; }
            get { return _gravitycomp; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Therapist
        {
            set { _therapist = value; }
            get { return _therapist; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BaseFileName
        {
            set { _basefilename = value; }
            get { return _basefilename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DataFileName
        {
            set { _datafilename = value; }
            get { return _datafilename; }
        }

        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
            }
        }

        public string ActionID
        {
            get
            {
                return _actionID;
            }
            set
            {
                _actionID = value;
            }
        }
        #endregion Model

        #region 实际值
        /// <summary>
        /// 测试关节
        /// </summary>
        public string DJoint
        {
            get;
            set;
        }
        /// <summary>
        /// 测试模式
        /// </summary>
        public string DTestMode
        {
            get;
            set;
        }
        /// <summary>
        /// 测试侧
        /// </summary>
        public string DJointSide
        {
            get;
            set;
        }
        /// <summary>
        /// 运动方式
        /// </summary>
        public string DPlane
        {
            get;
            set;
        }
        /// <summary>
        /// 受伤侧
        /// </summary>
        public string DInsuredSide
        {
            get;
            set;
        }
        /// <summary>
        /// 是否重力补偿
        /// </summary>
        public string DGravitycomp
        {
            get;
            set;
        }

        /// <summary>
        /// 运动范围
        /// </summary>
        public string MotionRange {
            get {
                return Motion_Start + "°- " + Motion_End + "°";
            }
        }

        /// <summary>
        /// 测试速度
        /// </summary>
        public string Speed {
            get {
                return Speed1 + "°/s- " + Speed2 + "°/s";
            }
        }
        #endregion

        public object Clone()
        {
            TestInfoModel tim = new TestInfoModel();
            tim.Acceleration1 = this.Acceleration1;
            tim.Acceleration2 = this.Acceleration2;
            tim.ActionID = this.ActionID;
            tim.Ath_Birthday = this.Ath_Birthday;
            tim.Ath_Code = this.Ath_Code;
            tim.Ath_Height = this.Ath_Height;
            tim.Ath_ID = this.Ath_ID;
            tim.Ath_Level = this.Ath_Level;
            tim.Ath_MainProject = this.Ath_MainProject;
            tim.Ath_Name = this.Ath_Name;
            tim.Ath_PinYin = this.Ath_PinYin;
            tim.Ath_Project = this.Ath_Project;
            tim.Ath_Remark = this.Ath_Remark;
            tim.Ath_Sex = this.Ath_Sex;
            tim.Ath_Team = this.Ath_Team;
            tim.Ath_TestAddress = this.Ath_TestAddress;
            tim.Ath_TestDate = this.Ath_TestDate;
            tim.Ath_TestID = this.Ath_TestID;
            tim.Ath_TestMachine = this.Ath_TestMachine;
            tim.Ath_TestState = this.Ath_TestState;
            tim.Ath_TrainYears = this.Ath_TrainYears;
            tim.Ath_Weight = this.Ath_Weight;
            tim.AthID = this.AthID;
            tim.BaseFileName = this.BaseFileName;
            tim.Break = this.Break;
            tim.DataFileName = this.DataFileName;
            tim.DGravitycomp = this.DGravitycomp;
            tim.DInsuredSide = this.DInsuredSide;
            tim.DJoint = this.DJoint;
            tim.DJointSide = this.DJointSide;
            tim.DPlane = this.DPlane;
            tim.DTestMode = this.DTestMode;
            tim.Gravitycomp = this.Gravitycomp;
            tim.Index = this.Index;
            tim.InsuredSide = this.InsuredSide;
            tim.IsChecked = this.IsChecked;
            tim.Joint = this.Joint;
            tim.Joint_Side = this.Joint_Side;
            tim.Motion_End = this.Motion_End;
            tim.Motion_Start = this.Motion_Start;
            tim.NOOfRepetitions = this.NOOfRepetitions;
            tim.NOOfSets = this.NOOfSets;
            tim.Plane = this.Plane;
            tim.Remark = this.Remark;
            tim.Speed1 = this.Speed1;
            tim.Speed2 = this.Speed2;
            tim.Test_Mode = this.Test_Mode;
            tim.TestDate = this.TestDate;
            tim.TestID = this.TestID;
            tim.TestTime = this.TestTime;
            tim.Therapist = this.Therapist;
            return tim;
        }
    }
}
