using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace FocusIt
{
    struct ReportData
    {
        public ReportData(int init_try_count, int init_succ_count, int init_fail_count)
        {
            try_count_ = init_try_count;
            success_count_ = init_succ_count;
            fail_count_ = init_fail_count;
        }

        public enum Type
        {
            kTry = 0,
            kSuccess,
            kFail
        }

        public string GetStringData()
        {
            return String.Format("TryCount({0}), SuccessCount({1}), FailCount({2})\n", 
                try_count_, success_count_, fail_count_);
        }

        public int try_count_;
        public int success_count_;
        public int fail_count_;
    }
    
    public partial class FocusIt : Form
    {
        private const string kVersionNo = "0.1.13";

        [DllImport("user32")]
        private static extern int FlashWindow(System.IntPtr hWnd, int bInvert);

        enum FocusState
        {
            kFocusState = 0,
            kPrepareRestState,
            kRestState,
            kStupidState
        }

        private int total_focus_time_sec_ = 0;
        private int total_rest_time_sec_ = 0;
        private int total_stupid_time_sec_ = 0;

        private int focus_time_min_ = 25;
        private int focus_time_sec_ = 0;
        
        private int rest_time_min_ = 5;
        private int rest_time_sec_ = 0;

        private const int kOneMinSec = 60;

        private const string kTitle = "FocusIt";

        private string prepare_rest_title_str = "휴식시간!!";
        private string prepare_rest_msg_str = "쉬어야 합니다.\n집중과 휴식의 적절한 조화가 필요합니다.\n쉬지 않고 일해서 열심히 일하고 있다고 느낄수 있지만,\n자신도 모르게 집중하는 시간은 줄어들고 있습니다 :)";

        private string focus_cancel_bt_str = "집중\n취소";
        private string rest_cancel_bt_str = "휴식\n취소";
        private string focus_start_bt_str = "집중\n시작";

        private string program_start_msg = "* {0} 프로그램 시작!, ver({1})";
        private string program_terminate_msg = "* {0} 프로그램 종료";

        private string change_focus_to_rest_msg = "집중 완료! 휴식 준비";
        private string change_stupid_state_msg = "대기 상태 시작";
        private string log_title_msg = "# Log [시:분:초] : Log Message\n";

        private string kRestTitle = "(휴식중)";
        private string kStandbyTitle = "(대기중)";

        private const string ini_file_name = ".\\" + kTitle + ".ini";

        private FocusState focus_state_;

        private int remain_total_sec_;

        private int remain_hour_;
        private int remain_min_;
        private int remain_sec_;

        private Timer focus_timer_;
        private Ini.IniUtil ini_util = new Ini.IniUtil();

        private StreamWriter log_stream_writer_;
        private FileStream log_file_stream_;

        private ReportData focus_report_data = new ReportData(0, 0, 0);

        public FocusIt()
        {
            InitializeComponent();

            LoadIniSetting();

            WriteLog(String.Format(program_start_msg, kTitle, kVersionNo));

            ChangeStupidState();
        }

        private void LoadIniSetting()
        {
            string str_path = Application.StartupPath;
            DateTime today_time = DateTime.Now;

            string log_file_name = ".//" + today_time.Year + "-" + today_time.Month + "-" + today_time.Day + ".txt";

            bool is_exist_today_log_file = File.Exists(log_file_name);
            if (is_exist_today_log_file == true)
            {
                log_file_stream_ = new FileStream(log_file_name, FileMode.Append, FileAccess.Write);
            }
            else
            {
                log_file_stream_ = new FileStream(log_file_name, FileMode.Create, FileAccess.Write);
            }

            log_stream_writer_ = new StreamWriter(log_file_stream_, System.Text.Encoding.UTF8);

            if (is_exist_today_log_file == false)
            {
                log_stream_writer_.Write(log_title_msg);
                log_stream_writer_.Flush();
            }

            bool is_exist_ini_file = File.Exists(ini_file_name);
            if (is_exist_ini_file == true)
            {
                focus_time_min_ = Convert.ToInt32(ini_util.GetIniValue("Time", "focus_min", ini_file_name));
                focus_time_sec_ = Convert.ToInt32(ini_util.GetIniValue("Time", "focus_sec", ini_file_name));

                rest_time_min_ = Convert.ToInt32(ini_util.GetIniValue("Time", "rest_min", ini_file_name));
                rest_time_sec_ = Convert.ToInt32(ini_util.GetIniValue("Time", "rest_sec", ini_file_name));
            }
        }

        private void SaveIniSetting()
        {
            string str_path = Application.StartupPath;

            ini_util.SetIniValue("Time", "focus_min", focus_time_min_.ToString(), ini_file_name);
            ini_util.SetIniValue("Time", "focus_sec", focus_time_sec_.ToString(), ini_file_name);

            ini_util.SetIniValue("Time", "rest_min", rest_time_min_.ToString(), ini_file_name);
            ini_util.SetIniValue("Time", "rest_sec", rest_time_sec_.ToString(), ini_file_name);

        }

        private String CalculateTime(int original_time_sec)
        {
            int total_hour = 0;
            int total_min = original_time_sec / kOneMinSec;
            int total_sec = original_time_sec - (total_min * kOneMinSec);
            if (total_min >= kOneMinSec)
            {
                total_hour = total_min / kOneMinSec;
                total_min -= (total_hour * kOneMinSec);
            }

            return String.Format("{0:00}시간 {1:00}분 {2:00}초", total_hour, total_min, total_sec);
        }

        private void ReportFocusData()
        {
            log_stream_writer_.Write("----------------------------------------\n");
            log_stream_writer_.Write("Report Focus Result\n");
            log_stream_writer_.Write(focus_report_data.GetStringData());
            log_stream_writer_.Write("Total Focus Time: " + CalculateTime(total_focus_time_sec_) + "\n");
            log_stream_writer_.Write("Total  Rest Time: " + CalculateTime(total_rest_time_sec_) + "\n");
            log_stream_writer_.Write("Total Enjoy Time: " + CalculateTime(total_stupid_time_sec_) + "\n");
            log_stream_writer_.Write("----------------------------------------\n");

            log_stream_writer_.Flush();
        }

        private void PrepareClosing()
        {
            if (focus_state_ == FocusState.kFocusState)
            {
                IncFocusReportDataCount(ReportData.Type.kFail);
                
                int total_focus_sec = focus_time_min_ * kOneMinSec + focus_time_sec_;
                int total_remain_focus_sec = remain_min_ * kOneMinSec + remain_sec_;

                total_focus_time_sec_ += total_focus_sec - total_remain_focus_sec;
            }
            else if (focus_state_ == FocusState.kRestState)
            {
                int total_rest_sec = rest_time_min_ * kOneMinSec + rest_time_sec_;
                int total_remain_rest_sec = remain_min_ * kOneMinSec + remain_sec_;

                total_rest_time_sec_ += total_rest_sec - total_remain_rest_sec;
            }
            else if (focus_state_ == FocusState.kStupidState)
            {
                total_stupid_time_sec_ += (remain_hour_ * kOneMinSec * kOneMinSec) + (remain_min_ * kOneMinSec) + remain_sec_;
            }            
        }   

        private void Focus_Closing(object sender, FormClosingEventArgs e)
        {
            PrepareClosing();
            
            SaveIniSetting();

            WriteLog(String.Format(program_terminate_msg, kTitle));

            ReportFocusData();

            log_stream_writer_.Close();
            log_file_stream_.Close();
        }

        private void SetFocusState(FocusState new_focus_state)
        {
            focus_state_ = new_focus_state;
        }

        private void SetButtonText(string bt_new_text)
        {
            this.focus_bt.Text = bt_new_text;
        }

        private void SetRemainTime(int remain_min, int remain_sec)
        {
            remain_min_ = remain_min;
            remain_sec_ = remain_sec;

            remain_total_sec_ = remain_min * kOneMinSec + remain_sec;
        }

        private void WriteLog(string log_msg)
        {
            DateTime cur_time = DateTime.Now;

            string log = String.Format("[{0:00}:{1:00}:{2:00}] : {3}\n", cur_time.Hour, cur_time.Minute, cur_time.Second, log_msg);

            log_stream_writer_.Write(log);
            log_stream_writer_.Flush();
        }

        private void IncFocusReportDataCount(ReportData.Type report_data_type)
        {
            switch (report_data_type)
            {
                case ReportData.Type.kTry:
                    focus_report_data.try_count_++;
                    break;
                case ReportData.Type.kSuccess:
                    focus_report_data.success_count_++;
                    break;
                case ReportData.Type.kFail:
                    focus_report_data.fail_count_++;
                    break;
            }
        }

        private void PreChangeFocusState()
        {
            total_stupid_time_sec_ += (remain_hour_ * kOneMinSec * kOneMinSec) + (remain_min_ * kOneMinSec) + remain_sec_;

            remain_hour_ = 0;

            int convert_remain_hour_to_min = remain_hour_ * kOneMinSec;
            WriteLog(String.Format("{0:00}분 {1:00}초 대기", convert_remain_hour_to_min + remain_min_, remain_sec_));            
            
        }

        private void ChangeFocusState()
        {
            PreChangeFocusState();

            this.Text = kTitle + "(집중!!)";
            time_label.ForeColor = Color.Black;

            SetFocusState(FocusState.kFocusState);
            SetButtonText(focus_cancel_bt_str);
            SetRemainTime(focus_time_min_, focus_time_sec_);
            
            WriteLog(String.Format("{0:00}분 {1:00}초 집중 시작", focus_time_min_, focus_time_sec_));            

            PlayBackwardProgress();

            IncFocusReportDataCount(ReportData.Type.kTry);
        }

        private void ChangePrepareRestState()
        {
            IncFocusReportDataCount(ReportData.Type.kSuccess);

            SetFocusState(FocusState.kPrepareRestState);

            FlashWindow(this.Handle, 1);

            WriteLog(change_focus_to_rest_msg);

            DialogResult result = MessageBox.Show(prepare_rest_msg_str, prepare_rest_title_str);
            if (result == DialogResult.OK)
            {
                FlashWindow(this.Handle, 0);
                ChangeRestState();
            }
        }

        private void ChangeRestState()
        {
            WriteLog(String.Format("{0:00}분 {1:00}초 휴식 시작", rest_time_min_, rest_time_sec_));            

            this.Text = kTitle + kRestTitle ;
            SetFocusState(FocusState.kRestState);
            SetButtonText(rest_cancel_bt_str);
            SetRemainTime(rest_time_min_, rest_time_sec_);

            PlayForwardProgress();
        }

        private void ChangeStupidState()
        {
            remain_min_ = 0;
            remain_sec_ = 0;

            this.Text = kTitle + kStandbyTitle;
            time_label.ForeColor = Color.Red;
            SetFocusState(FocusState.kStupidState);
            SetButtonText(focus_start_bt_str);

            WriteLog(change_stupid_state_msg);
        }

        private void PlayForwardProgress()
        {
            focus_progress.Maximum = remain_total_sec_;
            focus_progress.Minimum = 0;
            focus_progress.Step = 1;
            focus_progress.Value = 0;
        }

        private void PlayBackwardProgress()
        {
            focus_progress.Maximum = remain_total_sec_;
            focus_progress.Minimum = 0;
            focus_progress.Step = -1;
            focus_progress.Value = remain_total_sec_;
        }

        private void UpdateProgress()
        {
            if (focus_state_ != FocusState.kStupidState)
            {
                focus_progress.PerformStep();
            }
        }

        private void focus_bt_Click(object sender, EventArgs e)
        {
            switch (focus_state_)
            {
                case FocusState.kStupidState:
                    ChangeFocusState();
                    break;
                case FocusState.kFocusState:
                    {
                        int total_focus_sec = focus_time_min_ * kOneMinSec + focus_time_sec_;
                        int total_remain_focus_sec = remain_min_ * kOneMinSec + remain_sec_;

                        int cur_focusing_sec = total_focus_sec - total_remain_focus_sec;

                        total_focus_time_sec_ += cur_focusing_sec;

                        int cur_focus_min = cur_focusing_sec / kOneMinSec;
                        int cur_focus_sec = cur_focusing_sec - (cur_focus_min * kOneMinSec);

                        WriteLog(String.Format("{0:00}분 {1:00}초 집중 후 취소", cur_focus_min, cur_focus_sec));                        

                        IncFocusReportDataCount(ReportData.Type.kFail);
                        ChangeRestState();
                        FlashWindow(this.Handle, 0);
                    }
                    break;
                case FocusState.kRestState:
                    {
                        int total_rest_sec = rest_time_min_ * kOneMinSec + rest_time_sec_;
                        int total_remain_rest_sec = remain_min_ * kOneMinSec + remain_sec_;

                        int cur_resting_sec = total_rest_sec - total_remain_rest_sec;

                        total_rest_time_sec_ += cur_resting_sec;

                        int cur_rest_min = cur_resting_sec / kOneMinSec;
                        int cur_rest_sec = cur_resting_sec - (cur_rest_min * kOneMinSec);

                        WriteLog(String.Format("{0:00}분 {1:00}초 휴식 후 취소", cur_rest_min, cur_rest_sec));                        

                        ChangeStupidState();
                    }
                    break;
            }
        }

        private void StartTimer()
        {
            focus_timer_ = new Timer();
            focus_timer_.Interval = 1000;
            focus_timer_.Tick += new EventHandler(Focus_Timer);
            focus_timer_.Start();
        }

        private void StopTimer()
        {
            focus_timer_.Stop();
        }

        private void Focus_Timer(object sender, System.EventArgs e)
        {
            if(focus_state_ == FocusState.kStupidState)
            {
                if (remain_sec_ == kOneMinSec)
                {
                    remain_sec_ = 0;
                    remain_min_++;
                }
                else
                {
                    remain_sec_++;
                }

                if (remain_min_ >= kOneMinSec)
                {
                    remain_hour_++;
                    remain_min_ = 0;
                }

                FlashWindow(this.Handle, 1);
            }
            else
            {
                if(remain_sec_ == 0)
                {
                    if(remain_min_ > 0)
                    {
                        remain_min_--;
                        remain_sec_ = 59;
                    }
                    else
                    {
                        if(focus_state_ == FocusState.kFocusState)
                        {
                            total_focus_time_sec_ += (focus_time_min_ * kOneMinSec) + focus_time_sec_;
                            ChangePrepareRestState();
                        }
                        else if(focus_state_ == FocusState.kRestState)
                        {
                            total_rest_time_sec_ += (rest_time_min_ * kOneMinSec);
                            ChangeStupidState();
                        }
                        else if (focus_state_ == FocusState.kPrepareRestState)
                        {
                            total_focus_time_sec_++;
                        }
                    }
                }
                else
                {
                    remain_sec_--;
                }
            }
            
            UpdateProgress();
            Invalidate();
        }
        
        private void Focus_Paint(object sender, PaintEventArgs e)
        {
            if (remain_hour_ > 0)
            {
                this.time_label.Text = String.Format("{0:00}:{1:00}:{2:00}", remain_hour_, remain_min_, remain_sec_);
                this.time_label.Font = new System.Drawing.Font("돋움", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            }
            else
            {
                this.time_label.Text = String.Format("{0:00}:{1:00}", remain_min_, remain_sec_);
                this.time_label.Font = new System.Drawing.Font("돋움", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            }
        }

        private void Focus_Load(object sender, EventArgs e)
        {
            StartTimer();
        }
    }
}
